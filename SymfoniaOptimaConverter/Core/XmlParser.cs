using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SymfoniaOptimaConverter.Core
{
   class XmlParser
   {
      private LinkedList< XElement >         m_elements = new LinkedList<XElement>();
      private CultureInfo                    m_ci;

      /**
       * Constructor.
       * 
       * @param element
       */
      public XmlParser( XElement element )
      {
         m_elements.AddFirst( element );

         m_ci = GetCultureInfo();
      }

      /**
       * Returns a common definition of the CultureInfo class used throughout thins application.
       */
      public static CultureInfo GetCultureInfo()
      {
         CultureInfo ci = (CultureInfo)CultureInfo.CurrentCulture.Clone();
         ci.NumberFormat.CurrencyDecimalSeparator = ".";
         ci.NumberFormat.NumberDecimalSeparator = ".";

         return ci;
      }

      /**
       * Opens the specified tag for reading
       */
      public XmlParser BeginMandatory( string id )
      {
         XElement parentElem = m_elements.First.Value;
         XElement queriedElem = parentElem.Element( id );
         if ( queriedElem == null )
         {
            throw new ArgumentNullException( "Mandatory tag <" + id + "> is not defined" );
         }
         else 
         {
            m_elements.AddFirst( queriedElem );
         }

         return this;
      }

      /**
       * Exits an open tag.
       */
      public XmlParser EndLevel()
      {
         m_elements.RemoveFirst();
         if ( m_elements.Count <= 0 )
         {
            throw new ArgumentNullException( "Too many tags were closed by the parser" );
         }
         return this;
      }

      /**
       * Queries the value of a mandatory element. 
       * In case the element is not defined, an exception will be thrown.
       * 
       * @param id
       * @param outValue
       */
      public XmlParser GetMandatoryStr( string id, ref string outValue )
      {
         XElement parentElem = m_elements.First.Value;
         XElement queriedElem = parentElem.Element( id );
         if ( queriedElem == null )
         {
            throw new ArgumentNullException( "Mandatory tag <" + id + "> is not defined" );
         }

         outValue = queriedElem.Value;

         return this;
      }

      /**
       * Queries the value of an optional element. 
       * 
       * @param id
       * @param outValue
       */
      public XmlParser GetOptionalStr( string id, ref string outValue )
      {
         XElement parentElem = m_elements.First.Value;
         XElement queriedElem = parentElem.Element( id );
         if ( queriedElem != null )
         {
            outValue = queriedElem.Value;
         }
         
         return this;
      }

      /**
       * Queries the value of a mandatory float element. 
       * In case the element is not defined, an exception will be thrown.
       * 
       * @param id
       * @param outValue
       */
      public XmlParser GetMandatoryFloat( string id, ref float outValue )
      {
         XElement parentElem = m_elements.First.Value;
         XElement queriedElem = parentElem.Element( id );
         if ( queriedElem == null )
         {
            throw new ArgumentNullException( "Mandatory tag <" + id + "> is not defined" );
         }

         string valueStr = queriedElem.Value;         
         try
         {
            outValue = float.Parse( valueStr, NumberStyles.Any, m_ci );
         }
         catch( Exception ex )
         {
            throw new ArgumentNullException( "Mandatory tag <" + id + "> is ill defined {" + ex.Message + "}" );
         }

         return this;
      }

      /**
       * Queries the value of an optional element. 
       * 
       * @param id
       * @param outValue
       */
      public XmlParser GetOptionalFloat( string id, ref float outValue )
      {
         XElement parentElem = m_elements.First.Value;
         XElement queriedElem = parentElem.Element( id );
         if ( queriedElem != null )
         {
            string valueStr = queriedElem.Value;
            try
            {
               outValue = float.Parse( valueStr, NumberStyles.Any, m_ci );
            }
            catch( Exception )
            {
               // nothing to do here
            }
         }

         return this;
      }
   }
}
