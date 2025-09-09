using System.IO;

namespace DebugLogger;

public class Test
{
 public   string Doit()
   {
      string filepath= "D:\\temp\\index.html"; 
      return string.Concat("abc ", Path.GetFileName("D:\\temp\\index.html"), " der");
     
    
   }
   
   
}