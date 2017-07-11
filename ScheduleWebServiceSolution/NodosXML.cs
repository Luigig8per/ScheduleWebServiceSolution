using DataLayer;
using System;
using System.Collections.Generic;
using System.Globalization;
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
        public int tipoConsulta;
        static void Main(string[] args)
        {

           webReq p = new webReq();
            p.webRequest();
        }

        void webRequest()
        {
            WebRequest request = WebRequest.Create("http://xml.donbest.com/v2/schedule/?token=F-!--!!_-vV73-_M");
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

            iteraChild(xmlDoc.DocumentElement, 0, "", "");
        }

      

        private void iteraChild(XmlNode xmlNode, int queriesCount, string sportId, string leagueId)
        {

            queriesCount += 1;
            string query="", query2="";
            
            Dictionary<string, string> dictionary = new Dictionary<string, string>();

            string tableName;

            if (xmlNode.HasChildNodes)
            {
                //'because its from the root, all the child nodes gonna be checked'

                foreach (XmlNode xmlNode2 in xmlNode.ChildNodes)
                {

                    //Next lines defines ids of league and sport
                    if (xmlNode2.Name == "league")
                    {
                        leagueId = (xmlNode2.Attributes[0].Value);              
                    }

                    else if (xmlNode2.Name=="sport")
                    {
                        sportId = (xmlNode2.Attributes[0].Value);
                    }


                    if ((xmlNode2.Name == "lines"))
                    {
                   //Lines actually will not be saved, due to indications of Ezequiel
                    }

                    else if(xmlNode2.Name == "event")
                    {

                       dictionary= storeEventFields(xmlNode2);
                        query = "insert into event ";
                        query2 = "values ";

                        query += "(" + xmlNode.Name + "_id,";
                        query2 = asignQueryValue(query2, xmlNode.Name, xmlNode.Attributes[0].Value);

                        query = asignQuery1Column("sport_id", query);
                        query2 = asignQueryValue(query2, "sport_id", sportId);

                        query = asignQuery1Column("league_id", query);
                        query2 = asignQueryValue(query2, "league_id", leagueId);

                        foreach (var item in dictionary)
                        {
                            query = asignQuery1Column(item.Key, query);
                            query2 = asignQueryValue(query2, item.Key, item.Value);
                        }

                        asignFinalValuesToQueryes(query, query2, tipoConsulta);
                    }
                      
                    else

                    {
                        //Define queris concatenated from each field on XML, including attributes, values and childNodes
                       

                        tableName = this.renameTableNames(xmlNode2.Name);

                        query = "insert into " + tableName + ""; ;
                        query2 = "values ";


                        //'Define index id with id from fatherNode'
                        if (xmlNode2.Attributes != null)
                        {
                            if (!(xmlNode.Attributes.Count == 0))
                            {
                                query += "(" + xmlNode.Name + "_id,";
                                query2 = asignQueryValue(query2, xmlNode.Name, xmlNode.Attributes[0].Value);
                            }
                            else
                            {
                                //Need to asign id to the nodes that doesnt have id value, these are the next 2 lines. PENDING
                                query += "(";
                                query2 += "(";
                            }


                            if (xmlNode2.Attributes.Count == 0)
                            {
                                //'' As he doesn't have childs, his values should be on same table
                                if (grandChildNodesCount(xmlNode2) == 0)
                                {
                                    //Verify that is not the root
                                    if (xmlNode.Attributes.Count > 0)
                                    {
                                        //'' This one should be changed to uopdate father
                                        //query2 = asignQueryValue(query2, tableName, xmlNode2.InnerText.Replace("'", "''"));

                                        foreach (XmlAttribute xmlAttribute2 in xmlNode.Attributes)
                                        {

                                           //this code part is actually not executting due to problem of so many updates in line.
                                            //Console.Write(xmlNode.Name + " - " + xmlAttribute.Name + " - " + xmlAttribute.Value);
                                            query = updateQueryStart(query, xmlNode.Name);
                                            query2 = "";
                                            query2 = updateQueryValue(query2, xmlNode2.Name, xmlNode2.InnerText.Replace("'", "''"), xmlNode.Attributes[0].Value, xmlNode.Name);

                                        }
                                    }
                                    else
                                    {
                                        //Case is root, pending add this query
                                        query = asignQuery1Column(tableName, query);
                                        query2 = asignQueryValue(query2, tableName, xmlNode2.InnerText);
                                    }
                                }

                                else

                                    if (xmlNode2.Value != null)
                                {
                                    query = asignQuery1Column(tableName, query);
                                    //query2 += "'" + xmlNode2.Value.Replace("'", "''") + "',";
                                    query2 = asignQueryValue(query2, tableName, xmlNode2.Value).Replace("'", "''") + "',";
                                }
                                else
                                {

                                    //this OONE SHOULD NOT BE EXECUTED? TEST,
                                    query = asignQuery1Column(tableName, query);
                                    query2 = asignQueryValue(query2, tableName, xmlNode2.Value);
                                    //query2 += "'" + xmlNode2.Value + "',";
                                }

                                asignFinalValuesToQueryes(query, query2, tipoConsulta);
                            }
                            else
                            {
                                //if just have childs, but no more, its time to add to database
                                if (grandChildNodesCount(xmlNode2) == 0)
                                {
                                    if (xmlNode.Attributes.Count > 0)
                                    {
                                        //'' This one should be changed to uopdate father
                                        //query2 = asignQueryValue(query2, tableName, xmlNode2.InnerText.Replace("'", "''"));

                                        foreach (XmlAttribute xmlAttribute2 in xmlNode2.Attributes)
                                        {

                                            //Console.Write(xmlNode.Name + " - " + xmlAttribute.Name + " - " + xmlAttribute.Value);

                                            if (xmlNode2.Name == "participant")
                                                
                                            {
                                                query = asignQuery1Column(xmlAttribute2.Name, query);
                                                query2 = asignQueryValue(query2, xmlAttribute2.Name, xmlAttribute2.Value);
                                            }
                                            else
                                            {
                                                query = updateQueryStart(query, xmlNode.Name);
                                                query2 = "";
                                                query2 = updateQueryValue(query2, xmlNode2.Name + "_" + xmlAttribute2.Name, xmlAttribute2.Value.Replace("'", "''"), xmlNode.Attributes[0].Value, xmlNode.Name);

                                                asignFinalValuesToQueryes(query, query2, 2);
                                            }
                                        }

                                    }
                                }


                                //Case node still have grandChilds
                                else

                                {
                                    foreach (XmlAttribute xmlAttribute2 in xmlNode2.Attributes)
                                    {



                                        query = asignQuery1Column(xmlAttribute2.Name, query);

                                        //if ((xmlNode2.ChildNodes.Count == 1) && (xmlNode2.Attributes.Count == 0))
                                        //    //this never gonna happen, as it says attribs.coun=0

                                        //    query2 = asignQueryValue(query2, xmlAttribute2.Name, xmlNode2.InnerText.Replace("'", "''"));
                                        //else
                                        query2 = asignQueryValue(query2, xmlAttribute2.Name, xmlAttribute2.Value.Replace("'", "''"));

                                    }
                                }
                                asignFinalValuesToQueryes(query, query2, tipoConsulta);

                            }


                        }


                        //If node doesn't have attributes, then just go to his childs, with next statement, applies also if have attributes.
                        iteraChild(xmlNode2, queriesCount, sportId, leagueId);

                    }
                }
            }

            //Console.WriteLine(queriesCount + "lines executed");


        }

        private Dictionary<string, string>  addAttsToDictionary(XmlNode xmlNode, Dictionary<string, string> dictionary, Boolean child)
        {
            string xmlNodeName="";string sideValue;

            if (!(xmlNode.Name == "lines"))
            {
                if (xmlNode.Attributes.Count > 0)
                {
                    foreach (XmlAttribute xmlAttribute2 in xmlNode.Attributes)

                    {
                        if (child) 
                            //If its child, first the name of father will be writed
                        {

                            if ((xmlNode.Name) == "participant")
                            {
                                sideValue = this.sideValue(xmlNode);

                                if (!(sideValue==""))
                                
                                //To check if have attribute AWAY
                                {
                                    xmlNodeName = xmlNode.Name + "_" + sideValue + "" + "_";
                                    
                                }
                              
                                try
                                { 
                                 dictionary.Add(xmlNodeName + xmlAttribute2.Name, xmlAttribute2.Value);
                                }
                                catch
                                {
                                    Console.WriteLine("ya existe " + xmlNodeName + xmlAttribute2.Name, ", " + xmlAttribute2.Value);
                                }

                            }

                            else
                            {
                                dictionary.Add(xmlNode.Name + "_" + xmlAttribute2.Name, xmlAttribute2.Value);
                            }


                        }
                        else

                        {
                            dictionary.Add(xmlAttribute2.Name, xmlAttribute2.Value);

                        }
                    }

                    if ((xmlNode.Name) == "participant")
                    { 
                        if (xmlNode.HasChildNodes)
                    {
                        foreach (XmlNode xmlNode2 in xmlNode.ChildNodes)
                        {
                             if (!(xmlNode2.Name=="pitcher"))
                                { 
                            foreach (XmlAttribute xmlAttribute3 in xmlNode2.Attributes)
                            {
                                        try
                                        { 
                                dictionary.Add(xmlNodeName + xmlAttribute3.Name, xmlAttribute3.Value);
                                        }
                                        catch
                                        {
                                            Console.WriteLine("ya existe " + xmlNodeName + xmlAttribute3.Name, ", " + xmlAttribute3.Value);
                                        }

                            }
                                }

                             else
                                {
                                    foreach (XmlAttribute xmlAttribute3 in xmlNode2.Attributes)
                                    {
                                        try
                                        {
                                            dictionary.Add(xmlNodeName + xmlNode2.Name + "_" + xmlAttribute3.Name, xmlAttribute3.Value);
                                        }
                                        catch
                                        {
                                            Console.WriteLine("ya existe " + xmlNodeName + xmlNode2.Name + "_" + xmlAttribute3.Name, ", " + xmlAttribute3.Value);
                                        }

                                    }
                                }

                            }
                    }
                    }


                }
                else
                    dictionary.Add(xmlNode.Name, xmlNode.InnerText);

            }

            return dictionary;
        }

        private string sideValue(XmlNode xmlNode)
        {
            string res = "";

           foreach (XmlAttribute xmlAttribute2 in xmlNode.Attributes)
            {
                if ((xmlAttribute2.Name=="side"))
                {
                    res = xmlAttribute2.Value;
                }
            }
            return res;
        }


       Dictionary<string,string> storeEventFields(XmlNode xmlNode)
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();


           dictionary= addAttsToDictionary(xmlNode, dictionary, false);

            foreach (XmlNode xmlNode2 in xmlNode.ChildNodes )
            {
                dictionary = addAttsToDictionary(xmlNode2, dictionary, true);
              

            }

            return dictionary;

        }

        private string renameTableNames(string tableName)
        {
            //Some table like 'group' need to change name to avoid problems with sql
            if (tableName == "group")
            {
                tableName = "groups";
            }


            return tableName;
        }

        private int grandChildNodesCount(XmlNode xmlNode)
        {
           

            int grandChildCount = 0;

            if (xmlNode.HasChildNodes)
            {

                foreach (XmlNode node in xmlNode.ChildNodes)
                {
                    if (node.HasChildNodes)
                    {
                        grandChildCount += 1;


                    }
                    else
                    {

                        if (node.Attributes != null)
                        {
                            grandChildCount += node.Attributes.Count;
                        }
                    }

                }

            }


            return grandChildCount;

        }

        private string asignQuery1Column(string columnName, string query)
        {

            query += columnName + ",";

            return query;
        }

        private string asignQueryValue(string query, string columnName, string columnValue)
        {
            if ((columnName == "date") && columnValue.Length > 8)
            {
                columnValue = convertToEastern(columnValue);
            }

            query += "'" + columnValue + "',";

            //if (query.StartsWith("(") == false)
            //    query = "(" + query;

            if (!(query.StartsWith("values (")))
            {
                query = query.Replace("values", "values (");
            }

            tipoConsulta = 1;
            return query;
        }

        private string updateQueryValue(string query, string columnName, string columnValue, string column_id, string tableName)
        {
            if (tableName == "participant")
            {
                query += ("" + columnName + " = '" + columnValue + "' where rot= '" + column_id + "'");
            }
            else
            {
                query += ("" + columnName + " = '" + columnValue + "' where id= '" + column_id + "'");
            }


            tipoConsulta = 2;
            return query;
        }

        private string updateQueryStart(string query, string tableName)
        {
            if (tableName == "participant")
            {

            }
            query = "update " + tableName + " set ";

            return query;
        }

        string asignFinalValuesToQueryes(string query1, string query2, int tipoConsulta)

            //If tipoConsulta=1 then its insert, if not, its update

        {
            string finalQuery;
            if (tipoConsulta == 1)
            {
                query1 += "timeReceived) ";
                query2 += "getDate())";

                query1 = query1.Replace(",)", ")");
                query2 = query2.Replace(",)", ")");

            }

            finalQuery = (query1 + " " + query2);

            Console.WriteLine(finalQuery);
            try
            {
                doQuery(finalQuery);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Insert error:" + ex.Message);
            }




            return (finalQuery);


        }

        object doQuery(string query)
        {
            Dbconnection dbCon = new Dbconnection();


            return dbCon.ExeScalar(query);



        }

        public string convertToEastern(string originalTime)
        {


            var localTime = DateTimeOffset.Parse(originalTime).UtcDateTime;
            TimeZoneInfo easternTimeZone = TimeZoneInfo.FindSystemTimeZoneById(
                                 "Eastern Standard Time");


            //DateTime dt = DateTime.ParseExact(originalTime, "yyyy-MM-dd'T'HH:mm:ssK",
            //                     CultureInfo.InvariantCulture,
            //                     DateTimeStyles.AdjustToUniversal);

            DateTime easternDateTime = TimeZoneInfo.ConvertTimeFromUtc(localTime,
                                                                       easternTimeZone);
            return easternDateTime.ToString();
        }






    }
}

