using DataLayer;
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
            Console.WriteLine("end");
            Console.WriteLine("end");

        }



        private void iteraChild(XmlNode xmlNode)
        {

            string query, query2;

            int cont = 0;
            string tableName;

            if (xmlNode.HasChildNodes)
            {
                //'because its from the root, all the child nodes gonna be checked'

                foreach (XmlNode xmlNode2 in xmlNode.ChildNodes)
                {

                    if (!(xmlNode2.Name == "lines"))
                    {
                        //Define queris concatenated from each field on XML, including attributes, values and childNodes

                        if (xmlNode2.Name == "group")
                        {
                            tableName = "group1";

                        }
                        else
                        {
                            tableName = xmlNode2.Name;
                        }

                        query = "insert into ";
                        query2 = "values ";
                        query += tableName + "";
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
                                //Need to asign id to the nodes that doesnt have id value, these are the next 2 lines. PENDING
                                query += "(";
                                query2 += "(";
                            }



                        }


                        if (xmlNode2.Attributes != null)
                        {

                            if (xmlNode2.Attributes.Count == 0)
                            {

                                query += tableName + ",";

                                if (xmlNode2.ChildNodes.Count == 1)
                                    query2 += "'" + xmlNode2.InnerText.Replace("'", "''") + "',";
                                else

                                    if (xmlNode2.Value != null)
                                {

                                    query2 += "'" + xmlNode2.Value.Replace("'", "''") + "',";
                                }
                                else
                                {
                                    query2 += "'" + xmlNode2.Value + "',";
                                }

                                asignFinalValuesToQueryes(query, query2);
                            }
                            else
                            {
                                foreach (XmlAttribute xmlAttribute2 in xmlNode2.Attributes)
                                {

                                    //Console.Write(xmlNode.Name + " - " + xmlAttribute.Name + " - " + xmlAttribute.Value);
                                    query += xmlAttribute2.Name + ",";
                                    if ((xmlNode2.ChildNodes.Count == 1) && (xmlNode2.Attributes.Count == 0))
                                        //this never gonna happen
                                        query2 += "'" + xmlNode2.InnerText.Replace("'", "''") + "',";
                                    else

                                        query2 += "'" + xmlAttribute2.Value.Replace("'", "''") + "',";
                                }

                                asignFinalValuesToQueryes(query, query2);

                            }
                        }

                        //If node doesn't have attributes, then just go to his childs, with next statement, applies also if have attributes.

                        iteraChild(xmlNode2);

                    }
                }
            }


        }

        


        string asignFinalValuesToQueryes(string query1, string query2)
        {
            string finalQuery;
            query1 += "timeReceived) ";
            query2 += "getDate())";

            query1 = query1.Replace(",)", ")");
            query2 = query2.Replace(",)", ")");

            finalQuery = (query1 + " " + query2);

            doQuery(finalQuery);
            Console.WriteLine(finalQuery);

            return (finalQuery);


        }

        object doQuery(string query)
        {
            Dbconnection dbCon = new Dbconnection();


            return dbCon.ExeScalar(query);



        }






    }
}

