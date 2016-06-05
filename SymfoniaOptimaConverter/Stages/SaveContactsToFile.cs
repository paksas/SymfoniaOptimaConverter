using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymfoniaOptimaConverter.Stages
{
   class SaveContactsToFile : SymfoniaOptimaConverter.Core.Stage
   {
      public override bool Activate(Form1 form)
      {
         form.optimaArchive.SaveContractors( form.outputDir.FullName, form.log );

         return true;
      }

      public override void Deactivate(Form1 form)
      {
      }

      public override SymfoniaOptimaConverter.Core.StageResult Tick(Form1 form)
      {
         return SymfoniaOptimaConverter.Core.StageResult.Completed;
      }
   }
}
