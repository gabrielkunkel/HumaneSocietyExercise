using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumaneSociety
{
    public static class Clean
    {


        public static string CleanString(string input)
        {
            string result = "";
            char[]letter = input.ToCharArray();
            foreach(char e in letter)
            {
                
                result += IsGood(e) ? e.ToString() : "" ;
            }

            return result;
        }


        private static bool IsGood(char input)
        {
           

            char[] charsToRemove = new char[] { '/', '\"', '*', '\\' };
            for ( int i = 0; i < charsToRemove.Length; i++)
            {
                if (charsToRemove[i] == input)
                {
                    return false;
                }
              
            }
            return true;
           
        }

        

        public static bool StringToBool(string input)
        {
            return input == "1" ;
        }

        public static string CheckNullString(string input)
        {
            if (input.ToLower() == "null")
            {
                return null;
            }
            else
            {
                return input;
            }
        }





    }

 


}
