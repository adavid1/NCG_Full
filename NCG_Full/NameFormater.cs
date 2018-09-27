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
                    break;
                case "Vlog No Copyright Music":
                    formatedName = nonFormatedName.Split('(')[0] + "[Chill]";
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
                        formatedName = nonFormatedName + "[Trap]";
                    }
                    break;
                case "Proximity":
                    if (nonFormatedName.Contains('['))
                    {
                        formatedName = nonFormatedName.Split('[')[0] + "[Electro∖Chill]";
                    }
                    else
                    {
                        formatedName = nonFormatedName + "[Electro∖Chill]";
                    }
                    break;
                case "xKito Music":
                    formatedName = nonFormatedName + " [Electro∖Chill]";
                    break;
            }

            return formatedName;
        }
    }
}