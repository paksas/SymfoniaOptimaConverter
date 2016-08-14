using Microsoft.VisualStudio.TestTools.UnitTesting;
using SymfoniaOptimaConverter.Optima.RegularInvoice;
using Moq;

namespace UnitTests.Optima
{
   [TestClass]
   public class HeaderTests
   {
      #region Invoice category

      [TestMethod]
      public void tax_free_invoice_consists_exclusively_of_tax_free_items()
      {
         // Arrange
         Header target = new Header();

         // Arrange
         Item[] itemsArr = new Item[] {
            new Item { m_isTaxFree = true },
            new Item { m_isTaxFree = true },
            new Item { m_isTaxFree = true },
         };
         
         // Act
         foreach( Item item in itemsArr )
         {
            target.Process(item);
         }

         // Assert
         Assert.IsTrue(target.m_isInvoiceTaxFree);
      }

      [TestMethod]
      public void single_non_tax_free_item_renders_invoice_non_tax_free()
      {
         // Arrange
         Header target = new Header();

         // Arrange
         Item[] itemsArr = new Item[] {
            new Item { m_isTaxFree = true },
            new Item { m_isTaxFree = false },
            new Item { m_isTaxFree = true },
         };

         // Act
         foreach (Item item in itemsArr)
         {
            target.Process(item);
         }

         // Assert
         Assert.IsFalse(target.m_isInvoiceTaxFree);
      }

      #endregion
   }
}
