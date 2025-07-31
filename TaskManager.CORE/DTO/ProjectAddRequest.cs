using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Core.DTO
{
   public record ProjectAddRequest( 
         string? ProjectName,
          string? ProjectDescription,
          DateTime? DateOfStart ,
          int TeamSize )
    {
        public ProjectAddRequest() : this( default,default,default,default)
        {
            
        }
    }
    
}
