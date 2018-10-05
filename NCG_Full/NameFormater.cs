using System.Linq;

namespace NCG_Full
{
    class NameFormater
    {
        public static string FormatByChannel(string nonFormatedName, string channelName)
        {
            string formatedName = "";

            switch (channelName)
            {
                case "Vital EDM":
                    formatedName = nonFormatedName.Split(']')[1].Substring(1) + " [Electro∖Dubstep]";
                    formatedName.Replace(" (Vital Release)", "");
                    break;
                case "Arc North":
                    if (nonFormatedName.Contains('['))
                    {
                        formatedName = nonFormatedName.Split('[')[0] + "[Electro∖Dubstep]";
                    }
                    else
                    {
                        formatedName = nonFormatedName + " [Electro∖Dubstep]";
                    }
                    break;
                case "GANGSTER GANG":
                    formatedName = nonFormatedName + " [Rap∖Hip-Hop]";
                    break;
                case "Trap Monkey":
                    if (nonFormatedName.Contains('['))
                    {
                        formatedName = nonFormatedName;
                    }
                    else
                    {
                        formatedName = nonFormatedName + " [Trap]";
                    }
                    break;
                case "Simplify.":
                    formatedName = nonFormatedName.Split('[')[0] + "[Electro∖Dubstep]";
                    break;
                case "Trap City":
                    if (nonFormatedName.Contains('('))
                    {
                        formatedName = nonFormatedName.Split('(')[0] + "[Trap]";
                    }
                    else
                    {
                        formatedName = nonFormatedName + " [Trap]";
                    }
                    break;
                case "Proximity":
                    if (nonFormatedName.Contains('['))
                    {
                        formatedName = nonFormatedName.Split('[')[0] + "[Electro∖Chill]";
                    }
                    else
                    {
                        formatedName = nonFormatedName + " [Electro∖Chill]";
                    }
                    break;
                case "xKito Music":
                    formatedName = nonFormatedName + " [Electro∖Chill]";
                    break;
                case "House Nation":
                    if (nonFormatedName.Contains('['))
                    {
                        formatedName = nonFormatedName.Split('[')[0] + "[Electro∖House]";
                    }
                    else
                    {
                        formatedName = nonFormatedName + " [Electro∖House]";
                    }
                    break;
                case "EDM Bot":
                        formatedName = nonFormatedName + " [Trap∖EDM]";
                    break;
                case "Trap Music Now.":
                    if (nonFormatedName.Contains('['))
                    {
                        formatedName = nonFormatedName.Split('[')[0] + "[Trap]";
                    }
                    else
                    {
                        formatedName = nonFormatedName + " [Trap]";
                    }
                    break;
                case "TrapMusicHDTV":
                    if (nonFormatedName.Contains('['))
                    {
                        formatedName = nonFormatedName.Split('[')[0] + "[Trap]";
                    }
                    else
                    {
                        formatedName = nonFormatedName + " [Trap]";
                    }
                    break;
                case "DJ Smile Music":
                    formatedName = nonFormatedName + " [Electro∖House]";
                    break;
                case "Indefinitely Music":
                    if (nonFormatedName.Contains("Lyrics"))
                    {
                        formatedName = nonFormatedName.Split('(')[0] + "[Pop∖House]";
                    }
                    else
                    {
                        formatedName = nonFormatedName + " [Pop∖House]";
                    }
                    break;
            }

            return formatedName;
        }
    }
}