using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SymfoniaOptimaConverter.Stages
{
   public class InputFileSelectionStage : SymfoniaOptimaConverter.Core.Stage
   {
      public override bool Activate( Form1 form )
      {
         DialogResult result = form.openFileDialog.ShowDialog();
         if ( result == DialogResult.OK )
         {            
            FileInfo fileInfo = new FileInfo( form.openFileDialog.FileName );
            DirectoryInfo inputDir = fileInfo.Directory;

            // create an output directory
            DirectoryInfo outputDir = null;
            try
            {
               outputDir = inputDir.CreateSubdirectory( "Optima" );

               WindowsIdentity id = WindowsIdentity.GetCurrent();
               var sid = new SecurityIdentifier(WellKnownSidType.AccountDomainUsersSid, id.User.AccountDomainSid);

               FileSystemAccessRule accessRule = new FileSystemAccessRule(sid,
                            FileSystemRights.FullControl,
                            InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit,
                            PropagationFlags.None,
                            AccessControlType.Allow );

               DirectorySecurity dirSec = outputDir.GetAccessControl();
               dirSec.AddAccessRule( accessRule );

               form.log.message( "Output directory " + outputDir + " created" );
               form.outputDir = outputDir;
            }
            catch( Exception ex )
            {
               string dirName = ( outputDir != null ) ? outputDir.ToString() : "<nonexisting dir>";
               form.log.error( "Can't create the output directory: " + dirName + " {" + ex.Message + "}" );
               return false;
            }

            return true;
         }
         else
         {
            return false;
         }
      }

      public override void Deactivate( Form1 form )
      {
      }

      public override SymfoniaOptimaConverter.Core.StageResult Tick( Form1 form )
      {
         // nothing to do here
         return SymfoniaOptimaConverter.Core.StageResult.Completed;
      }

   }
}
