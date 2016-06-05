using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace SymfoniaOptimaConverter.Stages
{
   public class UploadContactsToDB : SymfoniaOptimaConverter.Core.Stage
   {
      private Task m_dbConnectionTask;
      private DateTime m_lastTime;

      public override bool Activate(Form1 form)
      {
         m_dbConnectionTask = UploadContacts(form);
         form.progressBar.Minimum = 0;
         form.progressBar.Maximum = 64000;
         form.progressBar.Value = 0;

         m_lastTime = DateTime.Now;

         return true;
      }

      public override void Deactivate(Form1 form)
      {
      }

      public override SymfoniaOptimaConverter.Core.StageResult Tick(Form1 form)
      {
         int newValue = Math.Min(form.progressBar.Value + GetDeltaTime(), form.progressBar.Maximum);
         form.progressBar.Value = newValue;

         try
         {
            bool completed = m_dbConnectionTask.Wait(1);
            if (completed)
            {
               return SymfoniaOptimaConverter.Core.StageResult.Completed;
            }
         }
         catch (Exception ex)
         {
            // cancel the operation now
            string errorMsg = "Database transaction failed.\nDetails:\n" + ex.Message;
            MessageBox.Show(errorMsg);

            return SymfoniaOptimaConverter.Core.StageResult.Failed;
         }

         return SymfoniaOptimaConverter.Core.StageResult.InProgress;
      }

      private int GetDeltaTime()
      {
         DateTime now = DateTime.Now;
         TimeSpan span = now.Subtract(m_lastTime);
         double ms = span.TotalMilliseconds;
         return (int)ms;
      }

      static async Task UploadContacts(Form1 form)
      {
         CancellationTokenSource cancellationToken = new CancellationTokenSource();
         cancellationToken.CancelAfter(2000); // give up after 2 seconds

         form.log.message("----------------------------------------------");
         form.log.message("Uploading contractors");
         form.log.message("----------------------------------------------");

         using (SqlConnection connection = new SqlConnection(form.dbConnStr))
         {
            await connection.OpenAsync(cancellationToken.Token);

            List<Optima.Contractor> remainingContractors = new List<Optima.Contractor>();
            foreach (SymfoniaOptimaConverter.Optima.Contractor contractor in form.optimaArchive.contractors)
            {
               bool entryFound = false;
               try
               {
                  entryFound = await DoesContractorExist(connection, form.companyNameSQL, contractor);
               }
               catch (Exception ex)
               {
                  form.log.error("Error accessing information about contractor " + contractor.m_code + "; " + ex.Message);
                  remainingContractors.Add(contractor);
                  continue;
               }

               if (!entryFound)
               {
                  // the entry doesn't exist, so insert it
                  try
                  {
                     bool wasInserted = await InsertContact(connection, form.companyNameSQL, form.contractorCategory, contractor);
                     if (wasInserted)
                     {
                        form.log.message("contractor " + contractor.m_code + " inserted");
                     }
                     else
                     {
                        form.log.warning("contractor " + contractor.m_code + " couldn't be inserted, saving to a file");
                        remainingContractors.Add(contractor);
                     }
                  }
                  catch (Exception ex)
                  {
                     form.log.error("Error inserting information about contractor " + contractor.m_code + "; " + ex.Message);
                     form.log.error(ex.StackTrace);
                     remainingContractors.Add(contractor);
                  }
               }
               else
               {
                  form.log.message("contractor " + contractor.m_code + " already exists, skipping");
               }
            }

            connection.Close();

            // add the remaining contractors for manual processing
            form.optimaArchive.contractors = remainingContractors;
         }
      }

      static async Task<bool> DoesContractorExist(SqlConnection connection, string companyName, SymfoniaOptimaConverter.Optima.Contractor contractor)
      {
         bool exists = false;
         if ( contractor.hasNIPAssigned )
         {
            exists = await FindContractorBy( connection, companyName, "Knt_NipE", contractor.m_NIP );
            if ( exists )
            {
               return true;
            }
         }

         string code = SafeSubstring( contractor.m_code, 0, 20 );
         exists = await FindContractorBy( connection, companyName, "Knt_Kod", code );
         return exists;
      }

      static async Task<bool> FindContractorBy(SqlConnection connection, string companyName, string fieldName, string fieldValue )
      {
         // check if a contractor with the given NIP address exists
         string doesEntryExistSQL = string.Format(s_findContractorBy
            , companyName
            , fieldName
            , fieldValue );
         SqlCommand doesEntryExistCmd = new SqlCommand(doesEntryExistSQL, connection);

         bool entryFound = false;
         using (SqlDataReader entryReader = await doesEntryExistCmd.ExecuteReaderAsync())
         {
            if (await entryReader.ReadAsync())
            {
               entryFound = true;
            }
         }

         return entryFound;
      }

      static async Task<bool> InsertContact(SqlConnection connection, string companyName, int category, SymfoniaOptimaConverter.Optima.Contractor contractor)
      {
         DateTime now = DateTime.Now;

         string nowStr = now.ToString();
         string guid = Guid.NewGuid().ToString();

         // we need to split the name into up to 3 parts if one exceeds the length of 40
         string[] name = SplitContractorName( contractor.m_name );
         string code = SafeSubstring( contractor.m_code, 0, 20 );
         string street = SafeSubstring( contractor.m_street, 0, 40 );

         string insertContactSQL = string.Format(s_insertContactSQL
            , companyName
            , code
            , name[0]
            , name[1]
            , name[2]
            , contractor.m_country
            , street
            , contractor.m_houseNo
            , contractor.m_apartmentNo
            , contractor.m_city
            , contractor.m_postalCode
            , contractor.m_NIP
            , category
            , guid
            , nowStr
            );

         SqlCommand insertContactCmd = new SqlCommand(insertContactSQL, connection);
         int result = await insertContactCmd.ExecuteNonQueryAsync();
         return result > 0;
      }

      private static string s_findContractorBy = "select [Knt_KntId] from [{0}].[CDN].[Kontrahenci] where [{1}] = '{2}';";

      private static string s_insertContactSQL = "INSERT INTO [{0}].[CDN].[Kontrahenci]"
           + "([Knt_PodmiotTyp]"
           + ",[Knt_Kod]"
           + ",[Knt_GLN]"
           + ",[Knt_EAN]"
           + ",[Knt_Grupa]"
           + ",[Knt_Nazwa1]"
           + ",[Knt_Nazwa2]"
           + ",[Knt_Nazwa3]"
           + ",[Knt_Kraj]"
           + ",[Knt_Wojewodztwo]"
           + ",[Knt_Powiat]"
           + ",[Knt_Gmina]"
           + ",[Knt_Ulica]"
           + ",[Knt_NrDomu]"
           + ",[Knt_NrLokalu]"
           + ",[Knt_Miasto]"
           + ",[Knt_KodPocztowy]"
           + ",[Knt_Poczta]"
           + ",[Knt_Adres2]"
           + ",[Knt_NipKraj]"
           + ",[Knt_NipE]"
           + ",[Knt_Nip]"
           + ",[Knt_Regon]"
           + ",[Knt_Pesel]"
           + ",[Knt_Telefon1]"
           + ",[Knt_Telefon2]"
           + ",[Knt_TelefonSms]"
           + ",[Knt_Fax]"
           + ",[Knt_Email]"
           + ",[Knt_URL]"
           + ",[Knt_KrajISO]"
           + ",[Knt_Zezwolenie]"
           + ",[Knt_KodTransakcji]"
           + ",[Knt_BazaBR_GUID]"
           + ",[Knt_BNaID]"
           + ",[Knt_RachunekNr]"
           + ",[Knt_IBAN]"
           + ",[Knt_OsTytul]"
           + ",[Knt_OsPlec]"
           + ",[Knt_OsNazwisko]"
           + ",[Knt_OsKraj]"
           + ",[Knt_OsWojewodztwo]"
           + ",[Knt_OsPowiat]"
           + ",[Knt_OsGmina]"
           + ",[Knt_OsUlica]"
           + ",[Knt_OsNrDomu]"
           + ",[Knt_OsNrLokalu]"
           + ",[Knt_OsMiasto]"
           + ",[Knt_OsKodPocztowy]"
           + ",[Knt_OsPoczta]"
           + ",[Knt_OsAdres2]"
           + ",[Knt_OsTelefon]"
           + ",[Knt_OsGSM]"
           + ",[Knt_OsEmail]"
           + ",[Knt_Informacje]"
           + ",[Knt_Upust]"
           + ",[Knt_LimitFlag]"
           + ",[Knt_LimitKredytu]"
           + ",[Knt_LimitPrzeterKredytFlag]"
           + ",[Knt_LimitPrzeterKredytWartosc]"
           + ",[Knt_Ceny]"
           + ",[Knt_FplID]"
           + ",[Knt_MaxZwloka]"
           + ",[Knt_TerminPlat]"
           + ",[Knt_Termin]"
           + ",[Knt_KontoOdb]"
           + ",[Knt_KontoDost]"
           + ",[Knt_KatID]"
           + ",[Knt_KatZakID]"
           + ",[Knt_BlokadaDok]"
           + ",[Knt_LimitKredytuWal]"
           + ",[Knt_LimitKredytuWykorzystany]"
           + ",[Knt_NieRozliczac]"
           + ",[Knt_PodatekVat]"
           + ",[Knt_Finalny]"
           + ",[Knt_FinalnyWegiel]"
           + ",[Knt_Export]"
           + ",[Knt_Rodzaj]"
           + ",[Knt_Rodzaj_Dostawca]"
           + ",[Knt_Rodzaj_Odbiorca]"
           + ",[Knt_Rodzaj_Konkurencja]"
           + ",[Knt_Rodzaj_Partner]"
           + ",[Knt_Rodzaj_Potencjalny]"
           + ",[Knt_Medialny]"
           + ",[Knt_MalyPod]"
           + ",[Knt_Rolnik]"
           + ",[Knt_Nieaktywny]"
           + ",[Knt_Chroniony]"
           + ",[Knt_Opis]"
           + ",[Knt_OdbiorcaId]"
           + ",[Knt_OpiekunTyp]"
           + ",[Knt_OpiekunId]"
           + ",[Knt_OpiekunKsiegTyp]"
           + ",[Knt_OpiekunKsiegID]"
           + ",[Knt_OpiekunKsiegDomyslny]"
           + ",[Knt_OpiekunPiKTyp]"
           + ",[Knt_OpiekunPiKID]"
           + ",[Knt_OpiekunPiKDomyslny]"
           + ",[Knt_TerminZwrotuKaucji]"
           + ",[Knt_NaliczajPlatnosc]"
           + ",[Knt_ZakazDokumentowHaMag]"
           + ",[Knt_ZgodaNaEFaktury]"
           + ",[Knt_TypWymiany]"
           + ",[Knt_PowiazanyUoV]"
           + ",[Knt_MetodaKasowa]"
           + ",[Knt_UWDId]"
           + ",[Knt_UWDOddzial]"
           + ",[Knt_UWDEmail]"
           + ",[Knt_TS_Export]"
           + ",[Knt_ImportAppId]"
           + ",[Knt_ImportRowId]"
           + ",[Knt_OpeZalID]"
           + ",[Knt_StaZalId]"
           + ",[Knt_TS_Zal]"
           + ",[Knt_OpeModID]"
           + ",[Knt_StaModId]"
           + ",[Knt_TS_Mod]"
           + ",[Knt_GIDTyp]"
           + ",[Knt_GIDFirma]"
           + ",[Knt_GIDNumer]"
           + ",[Knt_GIDLp]"
           + ",[Knt_eSklepID]"
           + ",[Knt_FCzynnosci]"
           + ",[Knt_FCzesci]"
           + ",[Knt_KorKraj]"
           + ",[Knt_KorMiasto]"
           + ",[Knt_KorKodPocztowy]"
           + ",[Knt_KorPoczta]"
           + ",[Knt_KorUlica]"
           + ",[Knt_KorNrDomu]"
           + ",[Knt_KorNrLokalu]"
           + ",[Knt_NieNaliczajOdsetek]"
           + ",[Knt_ESklep]"
           + ",[Knt_ZwolnionyZAkcyzy]"
           + ",[Knt_WindykacjaSchematId]"
           + ",[Knt_WindykacjaOsobaId]"
           + ",[Knt_WindykacjaEMail]"
           + ",[Knt_WindykacjaTelefonSms]"
           + ",[Knt_Komornik]"
           + ",[Knt_KomornikOkreg]"
           + ",[Knt_KomornikMiasto]"
           + ",[Knt_KomornikRewir])"
     + "VALUES"
           + "( 1"            //	<Knt_PodmiotTyp, smallint,>
           + ",'{1}'"         //	<Knt_Kod, varchar(20),>
           + ",''"            //	<Knt_GLN, varchar(13),>
           + ",''"			   //<Knt_EAN, varchar(16),>
           + ",''"            // <Knt_Grupa, varchar(20),>"
           + ",'{2}'"			//<Knt_Nazwa1, nvarchar(50),>"
           + ",'{3}'"			   //<Knt_Nazwa2, nvarchar(50),>"
           + ",'{4}'"			   //<Knt_Nazwa3, nvarchar(250),>"
           + ",'{5}'"			//<Knt_Kraj, nvarchar(40),>"
           + ",''"            //<Knt_Wojewodztwo, nvarchar(40),>"
           + ",''"			   //<Knt_Powiat, nvarchar(40),>"
           + ",''"			   //<Knt_Gmina, nvarchar(40),>"
           + ",'{6}'"         //<Knt_Ulica, nvarchar(40),>"
           + ",'{7}'	"			//<Knt_NrDomu, nvarchar(10),>"
           + ",'{8}'"			//<Knt_NrLokalu, nvarchar(10),>"
           + ",'{9}'"			//<Knt_Miasto, nvarchar(40),>"
           + ",'{10}'"			//<Knt_KodPocztowy, varchar(10),>"
           + ",''"			   //<Knt_Poczta, nvarchar(40),>"
           + ",''"			   //<Knt_Adres2, nvarchar(40),>"
           + ",''"			   //<Knt_NipKraj, varchar(2),>"
           + ",'{11}'"         //<Knt_NipE, varchar(13),>"
           + ",'{11}'"         //<Knt_Nip, varchar(13),>"
           + ",''	"			   //<Knt_Regon, varchar(20),>"
           + ",''"            //<Knt_Pesel, nvarchar(11),>"
           + ",''"			   //<Knt_Telefon1, varchar(20),>"
           + ",''"			   //<Knt_Telefon2, varchar(20),>"
           + ",''"			   //<Knt_TelefonSms, varchar(20),>"
           + ",''"			   //<Knt_Fax, varchar(20),>"
           + ",''"			   //<Knt_Email, nvarchar(127),>"
           + ",''"			   //<Knt_URL, varchar(254),>"
           + ",''"			   //<Knt_KrajISO, nvarchar(9),>"
           + ",''"			   //<Knt_Zezwolenie, nvarchar(40),>"
           + ",''"			   //<Knt_KodTransakcji, nvarchar(2),>"
           + ",null"			   //<Knt_BazaBR_GUID, uniqueidentifier,>"
           + ",null"			   //<Knt_BNaID, int,>"
           + ",''"			   //<Knt_RachunekNr, nvarchar(51),>"
           + ",0"			      //<Knt_IBAN, smallint,>"
           + ",''"			   //<Knt_OsTytul, nvarchar(20),>"
           + ",0"			      //<Knt_OsPlec, tinyint,>"
           + ",''"			   //<Knt_OsNazwisko, nvarchar(40),>"
           + ",''"			   //<Knt_OsKraj, nvarchar(40),>"
           + ",''"			   //<Knt_OsWojewodztwo, nvarchar(40),>"
           + ",''"			   //<Knt_OsPowiat, nvarchar(40),>"
           + ",''"			   //<Knt_OsGmina, nvarchar(40),>"
           + ",''"			   //<Knt_OsUlica, nvarchar(40),>"
           + ",''"			   //<Knt_OsNrDomu, nvarchar(10),>"
           + ",''"			   //<Knt_OsNrLokalu, nvarchar(10),>"
           + ",''"			   //<Knt_OsMiasto, nvarchar(40),>"
           + ",''"			   //<Knt_OsKodPocztowy, varchar(10),>"
           + ",''"			   //<Knt_OsPoczta, nvarchar(40),>"
           + ",''"			   //<Knt_OsAdres2, nvarchar(40),>"
           + ",''"			   //<Knt_OsTelefon, varchar(20),>"
           + ",''"			   //<Knt_OsGSM, varchar(20),>"
           + ",''"			   //<Knt_OsEmail, nvarchar(40),>"
           + ",0"			      //<Knt_Informacje, tinyint,>"
           + ",0.0"			   //<Knt_Upust, decimal(5,2),>"
           + ",0"			      //<Knt_LimitFlag, tinyint,>"
           + ",0.0"			   //<Knt_LimitKredytu, decimal(15,2),>"
           + ",0"			      //<Knt_LimitPrzeterKredytFlag, tinyint,>"
           + ",0.0"			   //<Knt_LimitPrzeterKredytWartosc, decimal(15,2),>"
           + ",0"			      //<Knt_Ceny, smallint,>"
           + ",3"			      //<Knt_FplID, int,>"
           + ",0"			      //<Knt_MaxZwloka, smallint,>"
           + ",0"			      //<Knt_TerminPlat, tinyint,>"
           + ",0"			      //<Knt_Termin, smallint,>"
           + ",''"			   //<Knt_KontoOdb, nvarchar(50),>"
           + ",''"			   //<Knt_KontoDost, nvarchar(50),>"
           + ",{12}"		      //<Knt_KatID, int,>"
           + ",{12}"		      //<Knt_KatZakID, int,>"
           + ",0"			      //<Knt_BlokadaDok, tinyint,>"
           + ",''"			   //<Knt_LimitKredytuWal, varchar(3),>"
           + ",0.0"			   //<Knt_LimitKredytuWykorzystany, decimal(15,2),>"
           + ",0"			      //<Knt_NieRozliczac, tinyint,>"
           + ",0"			      //<Knt_PodatekVat, tinyint,>"
           + ",1"			      //<Knt_Finalny, tinyint,>"
           + ",0"			      //<Knt_FinalnyWegiel, tinyint,>"
           + ",0"			      //<Knt_Export, tinyint,>"
           + ",1"			      //<Knt_Rodzaj, tinyint,>"
           + ",1"			      //<Knt_Rodzaj_Dostawca, tinyint,>"
           + ",1"			      //<Knt_Rodzaj_Odbiorca, tinyint,>"
           + ",1"			      //<Knt_Rodzaj_Konkurencja, tinyint,>"
           + ",1"			      //<Knt_Rodzaj_Partner, tinyint,>"
           + ",0"			      //<Knt_Rodzaj_Potencjalny, tinyint,>"
           + ",0"			      //<Knt_Medialny, tinyint,>"
           + ",0"			      //<Knt_MalyPod, tinyint,>"
           + ",0"			      //<Knt_Rolnik, tinyint,>"
           + ",0"			      //<Knt_Nieaktywny, tinyint,>"
           + ",0"			      //<Knt_Chroniony, tinyint,>"
           + ",''"			   //<Knt_Opis, nvarchar(254),>"
           + ",null"			   //<Knt_OdbiorcaId, int,>"
           + ",null"			   //<Knt_OpiekunTyp, smallint,>"
           + ",null"			   //<Knt_OpiekunId, int,>"
           + ",null"			   //<Knt_OpiekunKsiegTyp, smallint,>"
           + ",null"			   //<Knt_OpiekunKsiegID, int,>"
           + ",1"			      //<Knt_OpiekunKsiegDomyslny, tinyint,>"
           + ",null"			   //<Knt_OpiekunPiKTyp, smallint,>"
           + ",null"			   //<Knt_OpiekunPiKID, int,>"
           + ",0"			      //<Knt_OpiekunPiKDomyslny, tinyint,>"
           + ",60"			   //<Knt_TerminZwrotuKaucji, smallint,>"
           + ",0"			      //<Knt_NaliczajPlatnosc, tinyint,>"
           + ",0"			      //<Knt_ZakazDokumentowHaMag, tinyint,>"
           + ",0"			      //<Knt_ZgodaNaEFaktury, tinyint,>"
           + ",0"			      //<Knt_TypWymiany, tinyint,>"
           + ",0"			      //<Knt_PowiazanyUoV, tinyint,>"
           + ",0"			      //<Knt_MetodaKasowa, tinyint,>"
           + ",0"			      //<Knt_UWDId, int,>"
           + ",''"			   //<Knt_UWDOddzial, nvarchar(10),>"
           + ",''"			   //<Knt_UWDEmail, nvarchar(30),>"
           + ",null"			   //<Knt_TS_Export, datetime,>"
           + ",null"			   //<Knt_ImportAppId, varchar(5),>"
           + ",'{13}'"			//<Knt_ImportRowId, varchar(36),>"
           + ",null"			   //<Knt_OpeZalID, int,>"
           + ",null"			   //<Knt_StaZalId, int,>"
           + ",'{14}'"			//<Knt_TS_Zal, datetime,>"
           + ",null"			   //<Knt_OpeModID, int,>"
           + ",null"			   //<Knt_StaModId, int,>"
           + ",'{14}'"			//<Knt_TS_Mod, datetime,>"
           + ",null"			   //<Knt_GIDTyp, smallint,>"
           + ",null"			   //<Knt_GIDFirma, int,>"
           + ",1"			      //<Knt_GIDNumer, int,>"
           + ",null"			   //<Knt_GIDLp, smallint,>"
           + ",null"			   //<Knt_eSklepID, int,>"
           + ",1"			      //<Knt_FCzynnosci, tinyint,>"
           + ",0"			      //<Knt_FCzesci, tinyint,>"
           + ",null"			   //<Knt_KorKraj, nvarchar(40),>"
           + ",null"			   //<Knt_KorMiasto, nvarchar(40),>"
           + ",null"			   //<Knt_KorKodPocztowy, varchar(10),>"
           + ",null"			   //<Knt_KorPoczta, nvarchar(40),>"
           + ",null"			   //<Knt_KorUlica, nvarchar(40),>"
           + ",null"			   //<Knt_KorNrDomu, nvarchar(10),>"
           + ",null"			   //<Knt_KorNrLokalu, nvarchar(10),>"
           + ",0"			      //<Knt_NieNaliczajOdsetek, tinyint,>"
           + ",0"			      //<Knt_ESklep, tinyint,>"
           + ",0"			      //<Knt_ZwolnionyZAkcyzy, tinyint,>"
           + ",null"			   //<Knt_WindykacjaSchematId, int,>"
           + ",null"			   //<Knt_WindykacjaOsobaId, int,>"
           + ",''"			   //<Knt_WindykacjaEMail, nvarchar(127),>"
           + ",''"			   //<Knt_WindykacjaTelefonSms, varchar(20),>"
           + ",0"			      //<Knt_Komornik, tinyint,>"
           + ",null"			   //<Knt_KomornikOkreg, nvarchar(256),>"
           + ",null"			   //<Knt_KomornikMiasto, nvarchar(40),>"
           + ",null"			   //<Knt_KomornikRewir, nvarchar(256),>"
           + ")";

      private static string[] SplitContractorName( string contractorName )
      {
         int[] fieldLengths = new int[]{ 50, 50, 250 };
         string[] name = new string[3];

         string analyzedString = contractorName;
         for ( int i = 0; i < fieldLengths.Length; ++i )
         {
            int fieldLength = fieldLengths[i];
            if ( analyzedString.Length > fieldLength )
            {
               string remainder = analyzedString.Substring( fieldLength );
               name[i] = SafeSubstring( analyzedString, 0, fieldLength );
               analyzedString = remainder;
            }
            else 
            {
               name[i] = analyzedString;
               break;
            }
         }

         return name;
      }

      private static string SafeSubstring( string inString, int startIdx, int length )
      {
         return inString.Substring( startIdx, Math.Min( inString.Length, length ) );
      }
   }

}
