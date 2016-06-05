using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymfoniaOptimaConverter.Core
{
   public enum StageResult
   {
      InProgress,
      Completed,
      Failed,
   }

   public abstract class Stage
   {         
      public abstract bool Activate( Form1 form );

      public abstract void Deactivate( Form1 form );

      public abstract StageResult Tick( Form1 form );
   }
}
