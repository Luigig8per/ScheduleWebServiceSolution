using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace wRequest
{
    class webReq
    {
        static void Main(string[] args)
        {


            webReq p = new webReq();
            p.webRequest();

        }

        void webRequest()
        {
            WebRequest request = WebRequest.Create("http://xml.donbest.com/v2/schedule/?token=mc7bB-!N5-BCA-Mn");

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            Console.WriteLine(response.StatusDescription);

            Stream dataStream = response.GetResponseStream();

            StreamReader reader = new StreamReader(dataStream);

            string responseFromServer = reader.ReadToEnd();

            reader.Close();
            dataStream.Close();
            response.Close();

            extractSports(responseFromServer);


        }

        private string loadXml(string text)
        {
            string res = "";
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(text);

            return res;


        }



        public void extractSports(string xmlText)
        {

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlText);


            iteraChild(xmlDoc.DocumentElement);


        }



        private void iteraChild(XmlNode xmlNode)
        {

            string query, query2;

            int cont = 0;


            if (xmlNode.HasChildNodes)
            {

                foreach (XmlNode xmlNode2 in xmlNode.ChildNodes)
                {
                    //if (!(xmlNode2.Name=="lines"))



                    {
                        query = "insert into ";
                        query2 = "values ";
                        query += xmlNode2.Name + "";
                        cont += 1;
                        //Console.WriteLine("Nodo " +  el.Name + ":" + el.Value + " " + el.Name);


                        //'Define index id with id from fatherNode'
                        if (xmlNode2.Attributes != null)
                        {
                            if (!(xmlNode.Attributes.Count == 0))
                            {
                                query += "(" + xmlNode.Name + "id,";
                                query2 += "('" + xmlNode.Attributes[0].Value + "',";
                            }
                            else
                            {
                                query += "(";
                                query2 += "(";
                            }



                        }


                        if (xmlNode2.Attributes != null)

                            if (xmlNode2.Attributes.Count == 0)
                            {


                                if ((xmlNode2.ChildNodes.Count == 1) && (xmlNode2.Attributes.Count == 0))
                                    query2 += "'" + xmlNode2.InnerText + "',";
                                else

                                    query2 += "'" + xmlNode2.Value + "',";

                                query += xmlNode2.Name + ",";


                                query += "timeReceived) ";
                                query2 += "getDate())";

                                query = query.Replace(",)", ")");
                                query2 = query2.Replace(",)", ")");

                                Console.WriteLine(query + " " + query2);
                            }
                            else
                            {
                                foreach (XmlAttribute xmlAttribute2 in xmlNode2.Attributes)
                                {

                                    //Console.Write(xmlNode.Name + " - " + xmlAttribute.Name + " - " + xmlAttribute.Value);
                                    query += xmlAttribute2.Name + ",";
                                    if ((xmlNode2.ChildNodes.Count == 1) && xmlNode2.Attributes.Count == 0)
                                        query2 += "'" + xmlNode2.InnerText + "',";
                                    else

                                        query2 += "'" + xmlAttribute2.Value + "',";
                                }

                                query += "timeReceived) ";
                                query2 += "getDate())";

                                query = query.Replace(",)", ")");
                                query2 = query2.Replace(",)", ")");

                                Console.WriteLine(query + " " + query2);





                            }

                        else
                        {

                        }

                        iteraChild(xmlNode2);

                    }
                }
            }


        }










    }
}

