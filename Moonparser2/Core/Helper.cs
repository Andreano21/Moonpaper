using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moonparser.Core
{
    public static class Helper
    {
        public static int ParseViews(string views)
        {
            string strViews = views;
            strViews.Trim();

            int StrToInt;

            bool isParsed = Int32.TryParse(strViews, out StrToInt);

            if (!isParsed)
            {
                //Удаление пробелов
                strViews = strViews.Replace(" ", "");
                strViews = strViews.Replace("K", "k");
                strViews = strViews.Replace(".", ",");

                
                float views_f;
                if (strViews.Contains(","))
                {
                    if (strViews.Contains("k"))
                    {
                        strViews = strViews.Replace("k", "");
                        float.TryParse(strViews, out views_f);
                        views_f *= 1000;
                        StrToInt = (int)views_f;
                    }
                    else
                    {
                        float.TryParse(strViews, out views_f);
                        StrToInt = (int)views_f;
                    }
                }
                else
                {
                    if (strViews.Contains("k"))
                    {
                        strViews = strViews.Replace("k", "000");
                        Int32.TryParse(strViews, out StrToInt);
                    }
                    else
                    {
                        Int32.TryParse(strViews, out StrToInt);
                    }
                }
            }
            else
            { 
                Int32.TryParse(strViews, out StrToInt);
            }


            return StrToInt;
        }
    }
}
