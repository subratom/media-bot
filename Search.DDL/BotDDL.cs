using Search.MediaStore.DataAccess;
using MySql.Data.MySqlClient;
using System;
using System.Net;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Search.MediaStore.Exceptions;
using Search.Helpers;
using Search.MediaStore.DDL.Helpers;
using System.Xml;
using Search.API.Classes;

namespace Search.MediaStore.DDL
{
    public class BotDDL
    {
        //private MySqlConnection _connection;
        public static string ConnectionString;

        public BotDDL()
        {
            //_connection = Data.GetConnection(ConnectionString);
        }

        public List<Mappings> ReprocessItems(string siteName, int MaxItems = 20)
        {
            //usp_ES_Update_UnprocessedUrls

            try
            {
                MySqlParameter[] param = new MySqlParameter[2];
                param[0] = new MySqlParameter
                {
                    Value = MaxItems,
                    ParameterName = "@count"
                };

                param[1] = new MySqlParameter
                {
                    Value = siteName,
                    ParameterName = "@siteName"
                };

                DataSet ds = Data.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, "usp_ES_Reprocess_ProcessedUrls", param);

                if (ds != null && ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        List<Mappings> itemList = new List<Mappings>();
                        foreach (DataRow item in ds.Tables[0].Rows)
                        {
                            Mappings currentItem = new Mappings
                            {
                                Id = Int32.Parse(item["id"].ToString()),
                                SiteName = item["site"].ToString(),
                                Url = item["url"].ToString(),
                                IsPartProcessed = item["ispartprocessed"].ToString() == "0" ? false : true,
                                IsUrlProcessed = item["isurlprocessed"].ToString() == "0" ? false : true
                            };
                            DateTime.TryParse(item["datecreated"].ToString(), out DateTime DateCreated);
                            DateTime.TryParse(item["datemodified"].ToString(), out DateTime DateModified);
                            currentItem.DateCreated = DateCreated == null ? DateTime.Now : DateCreated;
                            currentItem.DateModified = DateModified == null ? DateTime.Now : DateModified;
                            currentItem.UrlMaxTries = Int32.Parse(item["url_max_tries"].ToString());
                            currentItem.PartMaxTries = Int32.Parse(item["part_max_tries"].ToString());
                            if (!string.IsNullOrEmpty(item["http_code"].ToString()))
                            {
                                currentItem.HttpCode = (HttpStatusCode)Enum.Parse(typeof(HttpStatusCode), item["http_code"].ToString());
                            }

                            if (currentItem.UrlMaxTries == 0)
                                itemList.Add(currentItem);

                        }

                        return itemList;
                    }
                }
                return null;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public List<Mappings> GetUrlsToProcess(int MaxItems = 100)
        {
            try
            {
                MySqlParameter[] param = new MySqlParameter[1];
                param[0] = new MySqlParameter
                {
                    Value = MaxItems,
                    ParameterName = "@count"
                };

                DataSet ds = Data.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, "usp_ES_Get_UnprocessedUrls", param);

                if (ds != null && ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        List<Mappings> itemList = new List<Mappings>();
                        foreach (DataRow item in ds.Tables[0].Rows)
                        {
                            Mappings currentItem = new Mappings
                            {
                                Id = Int32.Parse(item["id"].ToString()),
                                SiteName = item["site"].ToString(),
                                Url = item["url"].ToString(),
                                IsPartProcessed = item["ispartprocessed"].ToString() == "0" ? false : true,
                                IsUrlProcessed = item["isurlprocessed"].ToString() == "0" ? false : true
                            };
                            DateTime.TryParse(item["datecreated"].ToString(), out DateTime DateCreated);
                            DateTime.TryParse(item["datemodified"].ToString(), out DateTime DateModified);

                            currentItem.DateCreated = DateCreated == null ? DateTime.Now : DateCreated;
                            currentItem.DateModified = DateModified == null ? DateTime.Now : DateModified;

                            currentItem.UrlMaxTries = Int32.Parse(item["url_max_tries"].ToString());
                            currentItem.PartMaxTries = Int32.Parse(item["part_max_tries"].ToString());
                            if (!string.IsNullOrEmpty(item["http_code"].ToString()))
                            {
                                currentItem.HttpCode = (HttpStatusCode)Enum.Parse(typeof(HttpStatusCode), item["http_code"].ToString());
                            }

                            if (currentItem.UrlMaxTries == 0)
                                itemList.Add(currentItem);

                        }

                        return itemList;
                    }
                }
                return null;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public List<Mappings> GetUrlsToProccessBySiteName(string SiteName, int MaxItems = 100)
        {
            try
            {
                MySqlParameter[] param = new MySqlParameter[2];
                param[0] = new MySqlParameter
                {
                    Value = MaxItems,
                    ParameterName = "@count"
                };

                param[1] = new MySqlParameter
                {
                    Value = SiteName,
                    ParameterName = "@siteName"
                };

                DataSet ds = Data.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, "usp_ES_Get_UnprocessedUrlsBySite", param);

                if (ds != null && ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        List<Mappings> itemList = new List<Mappings>();
                        foreach (DataRow item in ds.Tables[0].Rows)
                        {
                            Mappings currentItem = new Mappings
                            {
                                Id = Int32.Parse(item["id"].ToString()),
                                SiteName = item["site"].ToString(),
                                Url = item["url"].ToString(),
                                IsPartProcessed = item["ispartprocessed"].ToString() == "0" ? false : true,
                                IsUrlProcessed = item["isurlprocessed"].ToString() == "0" ? false : true
                            };
                            DateTime.TryParse(item["datecreated"].ToString(), out DateTime DateCreated);
                            DateTime.TryParse(item["datemodified"].ToString(), out DateTime DateModified);

                            currentItem.DateCreated = DateCreated == null ? DateTime.Now : DateCreated;
                            currentItem.DateModified = DateModified == null ? DateTime.Now : DateModified;

                            currentItem.UrlMaxTries = Int32.Parse(item["url_max_tries"].ToString());
                            currentItem.PartMaxTries = Int32.Parse(item["part_max_tries"].ToString());
                            if (!string.IsNullOrEmpty(item["http_code"].ToString()))
                            {
                                currentItem.HttpCode = (HttpStatusCode)Enum.Parse(typeof(HttpStatusCode), item["http_code"].ToString());
                            }

                            if (currentItem.UrlMaxTries == 0)
                                itemList.Add(currentItem);

                        }

                        return itemList;
                    }
                }
                return null;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public List<Mappings> GetAllUrlsToProccessBySiteName(string SiteName, int MaxItems = 100)
        {
            try
            {
                MySqlParameter[] param = new MySqlParameter[2];
                param[0] = new MySqlParameter
                {
                    Value = MaxItems,
                    ParameterName = "@count"
                };

                param[1] = new MySqlParameter
                {
                    Value = SiteName,
                    ParameterName = "@siteName"
                };

                DataSet ds = Data.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, "usp_ES_Get_UnprocessedUrlsBySite", param);

                if (ds != null && ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        List<Mappings> itemList = new List<Mappings>();
                        foreach (DataRow item in ds.Tables[0].Rows)
                        {
                            Mappings currentItem = new Mappings
                            {
                                Id = Int32.Parse(item["id"].ToString()),
                                SiteName = item["site"].ToString(),
                                Url = item["url"].ToString(),
                                IsPartProcessed = item["ispartprocessed"].ToString() == "0" ? false : true,
                                IsUrlProcessed = item["isurlprocessed"].ToString() == "0" ? false : true
                            };
                            DateTime.TryParse(item["datecreated"].ToString(), out DateTime DateCreated);
                            DateTime.TryParse(item["datemodified"].ToString(), out DateTime DateModified);

                            currentItem.DateCreated = DateCreated == null ? DateTime.Now : DateCreated;
                            currentItem.DateModified = DateModified == null ? DateTime.Now : DateModified;

                            currentItem.UrlMaxTries = Int32.Parse(item["url_max_tries"].ToString());
                            currentItem.PartMaxTries = Int32.Parse(item["part_max_tries"].ToString());
                            if (!string.IsNullOrEmpty(item["http_code"].ToString()))
                            {
                                currentItem.HttpCode = (HttpStatusCode)Enum.Parse(typeof(HttpStatusCode), item["http_code"].ToString());
                            }

                            if (currentItem.UrlMaxTries == 0)
                                itemList.Add(currentItem);

                        }

                        return itemList;
                    }
                }
                return null;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public List<Mappings> GetEPUrlsToProcess(int MaxItems = 100)
        {
            try
            {
                MySqlParameter[] param = new MySqlParameter[1];
                param[0] = new MySqlParameter
                {
                    Value = MaxItems,
                    ParameterName = "@count"
                };

                DataSet ds = Data.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, "usp_ES_Get_EPUnprocessedUrls", param);

                if (ds != null && ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        List<Mappings> itemList = new List<Mappings>();
                        foreach (DataRow item in ds.Tables[0].Rows)
                        {
                            Mappings currentItem = new Mappings
                            {
                                Id = Int32.Parse(item["id"].ToString()),
                                SiteName = item["site"].ToString(),
                                Url = item["url"].ToString(),
                                IsPartProcessed = item["ispartprocessed"].ToString() == "0" ? false : true,
                                IsUrlProcessed = item["isurlprocessed"].ToString() == "0" ? false : true
                            };
                            DateTime.TryParse(item["datecreated"].ToString(), out DateTime DateCreated);
                            DateTime.TryParse(item["datemodified"].ToString(), out DateTime DateModified);

                            currentItem.DateCreated = DateCreated == null ? DateTime.Now : DateCreated;
                            currentItem.DateModified = DateModified == null ? DateTime.Now : DateModified;

                            currentItem.UrlMaxTries = Int32.Parse(item["url_max_tries"].ToString());
                            currentItem.PartMaxTries = Int32.Parse(item["part_max_tries"].ToString());
                            if (!string.IsNullOrEmpty(item["http_code"].ToString()))
                            {
                                currentItem.HttpCode = (HttpStatusCode)Enum.Parse(typeof(HttpStatusCode), item["http_code"].ToString());
                            }

                            if (currentItem.UrlMaxTries == 0)
                                itemList.Add(currentItem);

                        }

                        return itemList;
                    }
                }
                return null;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public List<Mappings> GetUrlsToProcessExternalLinks(int MaxItems)
        {
            try
            {
                MySqlParameter[] param = new MySqlParameter[1];
                param[0] = new MySqlParameter
                {
                    Value = MaxItems,
                    ParameterName = "@count"
                };

                DataSet ds = Data.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, "usp_EL_Get_UnprocessedUrls", param);

                if (ds != null && ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        List<Mappings> itemList = new List<Mappings>();
                        foreach (DataRow item in ds.Tables[0].Rows)
                        {
                            Mappings currentItem = new Mappings
                            {
                                Id = Int32.Parse(item["id"].ToString()),
                                SiteName = item["site"].ToString(),
                                Url = item["url"].ToString(),
                                IsPartProcessed = item["ispartprocessed"].ToString() == "0" ? false : true,
                                IsUrlProcessed = item["isurlprocessed"].ToString() == "0" ? false : true
                            };
                            DateTime.TryParse(item["datecreated"].ToString(), out DateTime DateCreated);
                            DateTime.TryParse(item["datemodified"].ToString(), out DateTime DateModified);

                            currentItem.UrlMaxTries = Int32.Parse(item["url_max_tries"].ToString());
                            currentItem.PartMaxTries = Int32.Parse(item["part_max_tries"].ToString());
                            if (!string.IsNullOrEmpty(item["http_code"].ToString()))
                            {
                                currentItem.HttpCode = (HttpStatusCode)Enum.Parse(typeof(HttpStatusCode), item["http_code"].ToString());
                            }
                            itemList.Add(currentItem);
                        }

                        return itemList;
                    }
                }
                return null;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public List<Mappings> GetPartsToProcess(int MaxItems)
        {
            try
            {
                MySqlParameter[] param = new MySqlParameter[1];
                param[0] = new MySqlParameter
                {
                    Value = MaxItems,
                    ParameterName = "@count"
                };

                DataSet ds = Data.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, "usp_ES_Get_UnprocessedParts", param);

                if (ds != null && ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        List<Mappings> itemList = new List<Mappings>();
                        foreach (DataRow item in ds.Tables[0].Rows)
                        {
                            Mappings currentItem = new Mappings
                            {
                                Id = Int32.Parse(item["id"].ToString()),
                                SiteName = item["site"].ToString(),
                                Url = item["url"].ToString(),
                                IsPartProcessed = item["ispartprocessed"].ToString() == "0" ? false : true,
                                IsUrlProcessed = item["isurlprocessed"].ToString() == "0" ? false : true
                            };
                            DateTime.TryParse(item["datecreated"].ToString(), out DateTime DateCreated);
                            DateTime.TryParse(item["datemodified"].ToString(), out DateTime DateModified);
                            currentItem.DateCreated = DateCreated == null ? DateTime.Now : DateCreated;
                            currentItem.DateModified = DateModified == null ? DateTime.Now : DateModified;

                            currentItem.UrlMaxTries = Int32.Parse(item["url_max_tries"].ToString());
                            currentItem.PartMaxTries = Int32.Parse(item["part_max_tries"].ToString());
                            //currentItem.HttpCode = (HttpStatusCode)Enum.Parse(typeof(HttpStatusCode), item["http_code"].ToString());

                            if (!string.IsNullOrEmpty(item["http_code"].ToString()))
                            {
                                currentItem.HttpCode = (HttpStatusCode)Enum.Parse(typeof(HttpStatusCode), item["http_code"].ToString());
                            }

                            itemList.Add(currentItem);
                        }

                        return itemList;
                    }
                }
                if (ds == null)
                    return null;
                else
                    return new List<Mappings>();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void InsertUrl(Mappings item)
        {
            try
            {
                if (IsUrlInDb(item.Url))
                {
                    //Data.ExecuteNonQuery(_connection, CommandType.StoredProcedure, "usp_ES_Update_Url", param);
                    //throw new ExceptionHandling("This Url cannot be inserted, item already exists in the database");
                }
                else
                {
                    MySqlParameter[] param = new MySqlParameter[10];
                    param[0] = new MySqlParameter
                    {
                        Value = item.SiteName,
                        ParameterName = "@siteName"
                    };

                    param[1] = new MySqlParameter
                    {
                        Value = item.Url,
                        ParameterName = "@validUrl"
                    };

                    param[2] = new MySqlParameter
                    {
                        Value = item.IsUrlProcessed == true ? 1 : 0,
                        ParameterName = "@isurlprocessed"
                    };

                    param[3] = new MySqlParameter
                    {
                        Value = item.IsPartProcessed == true ? 1 : 0,
                        ParameterName = "@ispartprocessed"
                    };

                    param[4] = new MySqlParameter
                    {
                        Value = item.UrlMaxTries,
                        ParameterName = "@url_max_tries"
                    };

                    param[5] = new MySqlParameter
                    {
                        Value = item.PartMaxTries,
                        ParameterName = "@part_max_tries"
                    };

                    param[6] = new MySqlParameter
                    {
                        Value = item.DateCreated,
                        ParameterName = "@datecreated"
                    };

                    param[7] = new MySqlParameter
                    {
                        Value = item.DateModified,
                        ParameterName = "@datemodified"
                    };

                    param[8] = new MySqlParameter
                    {
                        Value = item.HttpCode,
                        ParameterName = "@http_code"
                    };

                    param[9] = new MySqlParameter
                    {
                        Value = 1,
                        ParameterName = "@updated_url"
                    };

                    Uri uri = new Uri(item.Url);
                    if (uri.Scheme == "http")
                    {
                        item.Url = item.Url.Replace("http", "https");
                        //if (!IsUrlInDb(item.Url))
                        //{

                        //}
                        if (CheckObject(item))
                        {
                            if (!string.IsNullOrEmpty(ConnectionString))
                                Data.ExecuteNonQuery(ConnectionString, CommandType.StoredProcedure, "usp_ES_Put_Url", param);
                            else
                                throw new ExceptionHandling("Connection to the database is null, please check the connection before proceeding");
                        }
                        else
                        {
                            throw new ExceptionHandling("Site Name or Url is empty. Cannot process the item");
                        }
                    }
                    else
                    {
                        if (CheckObject(item))
                        {
                            if (!string.IsNullOrEmpty(ConnectionString))
                                Data.ExecuteNonQuery(ConnectionString, CommandType.StoredProcedure, "usp_ES_Put_Url", param);
                            else
                                throw new ExceptionHandling("Connection to the database is null, please check the connection before proceeding");
                        }
                        else
                        {
                            throw new ExceptionHandling("Site Name or Url is empty. Cannot process the item");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            //This will insert the item the item.
        }

        private bool CheckObject(Mappings item)
        {
            if (string.IsNullOrEmpty(item.Url))
            {
                throw new ExceptionHandling("Url is empty");
            }

            if (string.IsNullOrEmpty(item.SiteName))
            {
                throw new ExceptionHandling("Site name is empty");
            }
            return true;
        }

        public void UpdateUrl(Mappings item)
        {
            try
            {
                if (CheckObject(item))
                {

                    //using (MySqlConnection conn = new MySqlConnection(ConnectionString))
                    //{
                    //    using (MySqlCommand cmd = new MySqlCommand("usp_ES_Update_Url", conn))
                    //    {
                    //        MySqlParameter[] param = new MySqlParameter[9];
                    //        param[0] = new MySqlParameter();
                    //        param[0].Value = item.SiteName;
                    //        param[0].ParameterName = "@siteName";

                    //        param[1] = new MySqlParameter();
                    //        param[1].Value = item.Url;
                    //        param[1].ParameterName = "@validUrl";

                    //        param[2] = new MySqlParameter();
                    //        param[2].Value = item.IsUrlProcessed == true ? 1 : 0;
                    //        param[2].ParameterName = "@isurlprocessed";

                    //        param[3] = new MySqlParameter();
                    //        param[3].Value = item.IsPartProcessed == true ? 1 : 0;
                    //        param[3].ParameterName = "@ispartprocessed";

                    //        param[4] = new MySqlParameter();
                    //        param[4].Value = item.UrlMaxTries;
                    //        param[4].ParameterName = "@url_max_tries";

                    //        param[5] = new MySqlParameter();
                    //        param[5].Value = item.PartMaxTries;
                    //        param[5].ParameterName = "@part_max_tries";

                    //        param[6] = new MySqlParameter();
                    //        param[6].Value = item.DateCreated;
                    //        param[6].ParameterName = "@datecreated";

                    //        param[7] = new MySqlParameter();
                    //        param[7].Value = item.DateModified;
                    //        param[7].ParameterName = "@datemodified";

                    //        param[8] = new MySqlParameter();
                    //        param[8].DbType = DbType.String;
                    //        param[8].Value = item.HttpCode;
                    //        param[8].ParameterName = "@http_code";

                    //        cmd.CommandType = CommandType.StoredProcedure;
                    //        cmd.Parameters.AddRange(param);

                    //        cmd.ExecuteScalar();
                    //    }
                    //}


                    if (!string.IsNullOrEmpty(ConnectionString))
                    {
                        MySqlParameter[] param = new MySqlParameter[10];
                        param[0] = new MySqlParameter
                        {
                            Value = item.SiteName,
                            ParameterName = "@siteName"
                        };

                        param[1] = new MySqlParameter
                        {
                            Value = item.Url,
                            ParameterName = "@validUrl"
                        };

                        param[2] = new MySqlParameter
                        {
                            Value = item.IsUrlProcessed == true ? 1 : 0,
                            ParameterName = "@isurlprocessed"
                        };

                        param[3] = new MySqlParameter
                        {
                            Value = item.IsPartProcessed == true ? 1 : 0,
                            ParameterName = "@ispartprocessed"
                        };

                        param[4] = new MySqlParameter
                        {
                            Value = item.UrlMaxTries,
                            ParameterName = "@url_max_tries"
                        };

                        param[5] = new MySqlParameter
                        {
                            Value = item.PartMaxTries,
                            ParameterName = "@part_max_tries"
                        };

                        param[6] = new MySqlParameter
                        {
                            Value = item.DateCreated,
                            ParameterName = "@datecreated"
                        };

                        param[7] = new MySqlParameter
                        {
                            Value = item.DateModified,
                            ParameterName = "@datemodified"
                        };

                        param[8] = new MySqlParameter
                        {
                            DbType = DbType.String,
                            Value = item.HttpCode,
                            ParameterName = "@http_code"
                        };

                        param[9] = new MySqlParameter
                        {
                            Value = item.UpdatedUrl == true ? 1 : 0,
                            ParameterName = "@updated_url"
                        };

                        Data.ExecuteScalar(ConnectionString, CommandType.StoredProcedure, "usp_ES_Update_Url", param);

                    }
                    else
                    {
                        throw new ExceptionHandling("Connection to the database is null, please check the connectio before proceeding");
                    }
                }
                else
                {
                    throw new ExceptionHandling("Sitename or Url is empty. Cannot process the item");
                }

                //if (IsUrlInDb(item.Url))
                //{

                //}
                //else
                //{
                //    throw new ExceptionHandling("Url cannot be updated, this url doesn't exist. Please insert this URL");
                //}
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void UpdateExternalUrlFlag(string Url, bool status)
        {
            try
            {
                if (IsUrlInDb(Url))
                {
                    if (!string.IsNullOrEmpty(ConnectionString))
                    {
                        MySqlParameter[] param = new MySqlParameter[2];
                        param[0] = new MySqlParameter
                        {
                            Value = Url,
                            ParameterName = "@url"
                        };

                        param[1] = new MySqlParameter
                        {
                            Value = status == true ? 1 : 0,
                            ParameterName = "@isexternallinkprocessed"
                        };

                        Data.ExecuteScalar(ConnectionString, CommandType.StoredProcedure, "usp_EL_Update_UnprocessedUrls", param);
                    }
                    else
                    {
                        throw new ExceptionHandling("Connection to the database is null, please check the connectio before proceeding");
                    }
                }
                else
                {
                    throw new ExceptionHandling("Url cannot be updated, this url doesn't exist. Please insert this URL");
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void UpdateExternalUrlFlag(string Url, int status)
        {
            try
            {
                if (IsUrlInDb(Url))
                {
                    if (!string.IsNullOrEmpty(ConnectionString))
                    {
                        MySqlParameter[] param = new MySqlParameter[2];
                        param[0] = new MySqlParameter
                        {
                            Value = Url,
                            ParameterName = "@url"
                        };

                        param[1] = new MySqlParameter
                        {
                            Value = status,
                            ParameterName = "@isexternallinkprocessed"
                        };

                        Data.ExecuteScalar(ConnectionString, CommandType.StoredProcedure, "usp_EL_Update_UnprocessedUrls", param);
                    }
                    else
                    {
                        throw new ExceptionHandling("Connection to the database is null, please check the connectio before proceeding");
                    }
                }
                else
                {
                    throw new ExceptionHandling("Url cannot be updated, this url doesn't exist. Please insert this URL");
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void ResetProcessedUrls()
        {
            //This will set every IsUrlProcessed column to zero.

            try
            {
                if (string.IsNullOrEmpty(ConnectionString))
                {
                    throw new Exception("Connection wasn't initialized. Make sure database connection is open");
                }
                else
                {
                    int returnInt = Data.ExecuteNonQuery(ConnectionString, CommandType.StoredProcedure, "usp_ES_Reset_Urls");
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void ResetPartSearch()
        {
            try
            {
                if (string.IsNullOrEmpty(ConnectionString))
                {
                    throw new Exception("Connection wasn't initialized. Make sure database connection is open");
                }
                else
                {
                    int returnInt = Data.ExecuteNonQuery(ConnectionString, CommandType.StoredProcedure, "usp_ES_Reset_PartProcessing");
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public Mappings GetItemByUrl(string Url)
        {
            try
            {
                if (string.IsNullOrEmpty(ConnectionString))
                {
                    throw new Exception("Connection wasn't initialized. Make sure database connection is open");
                }
                else
                {
                    MySqlParameter[] param = new MySqlParameter[1];
                    param[0] = new MySqlParameter
                    {
                        Value = Url,
                        ParameterName = "@validUrl"
                    };


                    DataSet dataset = Data.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, "usp_ES_Get_Url", param);
                    if (dataset != null && dataset.Tables.Count > 0)
                    {
                        DataTable dataTable = dataset.Tables[0];
                        if (dataTable.Rows.Count > 1)
                        {
                            throw new Exception("More than one URL found. Cannot process");
                        }
                        else if (dataTable.Rows.Count == 1)
                        {
                            Mappings currentItem = new Mappings();
                            foreach (DataRow dr in dataTable.Rows)
                            {


                                currentItem.Id = Int32.Parse(dr["id"].ToString());
                                currentItem.SiteName = dr["site"].ToString();
                                currentItem.Url = dr["url"].ToString();
                                currentItem.IsPartProcessed = dr["ispartprocessed"].ToString() == "0" ? false : true;
                                currentItem.IsUrlProcessed = dr["isurlprocessed"].ToString() == "0" ? false : true;
                                DateTime.TryParse(dr["datecreated"].ToString(), out DateTime DateCreated);
                                DateTime.TryParse(dr["datemodified"].ToString(), out DateTime DateModified);

                                currentItem.UrlMaxTries = Int32.Parse(dr["url_max_tries"].ToString());
                                currentItem.PartMaxTries = Int32.Parse(dr["part_max_tries"].ToString());

                                //currentItem.HttpCode = (HttpStatusCode)Enum.Parse(typeof(HttpStatusCode), dr["http_code"].ToString());
                                if (!string.IsNullOrEmpty(dr["http_code"].ToString()))
                                {
                                    currentItem.HttpCode = (HttpStatusCode)Enum.Parse(typeof(HttpStatusCode), dr["http_code"].ToString());
                                }
                            }
                            return currentItem;
                        }
                        else
                        {
                            return null;
                        }
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        private bool IsUrlInDb(string Url)
        {

            bool isUrlInList = false;
            try
            {
                if (string.IsNullOrEmpty(ConnectionString))
                {
                    throw new Exception("Connection wasn't initialized. Make sure database connection is open");
                }
                else
                {

                    //using (MySqlConnection conn = new MySqlConnection(Connectionstring))
                    //{
                    //    using (MySqlCommand cmd = new MySqlCommand("usp_ES_Get_Url"))
                    //    {
                    //        MySqlParameter[] param = new MySqlParameter[1];
                    //        param[0] = new MySqlParameter();
                    //        param[0].Value = Url;
                    //        param[0].ParameterName = "@validUrl";

                    //        cmd.Parameters.AddRange(param);

                    //        cmd.ExecuteReader
                    //    }
                    //}

                    MySqlParameter[] param = new MySqlParameter[1];
                    param[0] = new MySqlParameter
                    {
                        Value = Url,
                        ParameterName = "@validUrl"
                    };


                    DataSet dataset = Data.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, "usp_ES_Get_Url", param);
                    if (dataset != null && dataset.Tables.Count > 0)
                    {
                        DataTable dataTable = dataset.Tables[0];
                        if (dataTable.Rows.Count > 1)
                        {
                            throw new Exception("More than one URL found. Cannot process");
                        }
                        else if (dataTable.Rows.Count == 1)
                        {
                            isUrlInList = true;
                        }
                        else
                        {
                            isUrlInList = false;
                        }
                    }
                    else
                    {
                        isUrlInList = false;
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return isUrlInList;
        }

        private string CleanUrl(string Url)
        {
            return Url;
        }

        public bool CreateMapping(string Url)
        {
            Mappings i = new Mappings
            {
                SiteName = this.CleanUrl(Url.GetSiteName()),
                Url = this.CleanUrl(Url),
                DateModified = null,
                IsUrlProcessed = false,
                IsPartProcessed = false,
                UrlMaxTries = 0,
                PartMaxTries = 0,
                DateCreated = null //This will need a value from somewhere. null value is incorrect
            };
            if (string.IsNullOrEmpty(i.Url))
                return false;
            else
            {
                this.InsertUrl(i);
                return true;
            }
        }

        public bool CreateMapping(XmlNode article)
        {
            Mappings i = new Mappings
            {
                SiteName = this.CleanUrl(XmlHelpers.GetXmlNodeValue(article, "loc")).GetSiteName()
            };
            ;
            i.Url = this.CleanUrl(XmlHelpers.GetXmlNodeValue(article, "loc"));
            i.DateModified = DateTime.Parse(XmlHelpers.GetXmlNodeValue(article, "lastmod"));
            i.IsUrlProcessed = false;
            i.IsPartProcessed = false;
            i.UrlMaxTries = 0;
            i.PartMaxTries = 0;
            i.DateCreated = DateTime.Now; //This will need a value from somewhere. null value is incorrect
            if (string.IsNullOrEmpty(i.Url))
                return false;
            else
            {
                this.InsertUrl(i);
                return true;
            }
        }

        public bool InsertExternalParentLinks(ExternalLinkParent item, List<string> Urls)
        {
            try
            {
                MySqlParameter[] param = new MySqlParameter[4];
                param[0] = new MySqlParameter
                {
                    Value = item.SiteName,
                    ParameterName = "@site_name"
                };

                param[1] = new MySqlParameter
                {
                    Value = item.SiteUrl,
                    ParameterName = "@site_url"
                };

                param[2] = new MySqlParameter
                {
                    Value = item.IsProcessed == true ? 1 : 0,
                    ParameterName = "@is_url_processed"
                };

                param[3] = new MySqlParameter
                {
                    Value = item.DateCreated,
                    ParameterName = "@date_created"
                };

                DataTable dataTable = IsExternalLinkInDb(item.SiteUrl);

                if (dataTable != null && dataTable.Rows.Count == 1)
                {
                    //Int64.Parse(dataTable.Rows[0][0].ToString());

                    //item.Id = int.Parse(dataTable.Rows[0][0].ToString());
                    //return DeleteAndReInsert(item, Urls);

                    throw new Exception("This Url cannot be inserted, item already exists in the database");
                    //May be add some code that will delete child items and reinsert them.
                }
                else
                {
                    if (!string.IsNullOrEmpty(ConnectionString))
                    {
                        Data.ExecuteNonQuery(ConnectionString, CommandType.StoredProcedure, "usp_EL_Put_ParentUrl", param);
                        if (!string.IsNullOrEmpty(ConnectionString))
                        {
                            return InsertChildItems(item, Urls);
                        }
                        else
                        {
                            throw new Exception("Connection to the database is null, please check the connection before proceeding");
                        }
                    }
                    else
                    {
                        throw new Exception("Connection to the database is null, please check the connection before proceeding");
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

            //InsertParentUrl
        }

        private bool DeleteAndReInsert(ExternalLinkParent item, List<string> Urls)
        {
            MySqlParameter[] param = new MySqlParameter[1];
            param[0] = new MySqlParameter
            {
                Value = item.Id,
                ParameterName = "@parent_id"
            };

            Data.ExecuteNonQuery(ConnectionString, CommandType.StoredProcedure, "usp_EL_Delete_ChildItems", param);

            return InsertChildItems(item, Urls);


        }

        private bool InsertChildItems(ExternalLinkParent item, List<string> Urls)
        {
            MySqlParameter[] param = new MySqlParameter[1];
            param[0] = new MySqlParameter
            {
                Value = item.SiteUrl,
                ParameterName = "@Url"
            };

            DataSet ds = Data.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, "usp_EL_Get_ParentUrl", param);

            if (ds.Tables.Count > 0)
            {
                DataRow r = ds.Tables[0].Rows[0];
                if (r != null && Int64.Parse(r["id"].ToString()) > 0)
                {
                    long parentId = Int64.Parse(r["id"].ToString());

                    foreach (string url in Urls)
                    {
                        param = new MySqlParameter[3];
                        param[0] = new MySqlParameter
                        {
                            Value = parentId,
                            ParameterName = "@parent_id"
                        };

                        param[1] = new MySqlParameter
                        {
                            Value = url,
                            ParameterName = "@url"
                        };

                        param[2] = new MySqlParameter
                        {
                            Value = DateTime.Now.Date,
                            ParameterName = "@date_created"
                        };

                        Data.ExecuteNonQuery(ConnectionString, CommandType.StoredProcedure, "usp_EL_Put_ChildUrls", param);
                    }
                    return true;
                }
                else
                {
                    throw new ExceptionHandling("Parent Url wasn't added in database");
                }
            }
            else
            {
                throw new ExceptionHandling("Parent Url wasn't added in database");
            }
        }

        private DataTable IsExternalLinkInDb(string Url)
        {
            try
            {
                if (string.IsNullOrEmpty(ConnectionString))
                {
                    throw new Exception("Connection wasn't initialized. Make sure database connection is open");
                }
                else
                {
                    MySqlParameter[] param = new MySqlParameter[1];
                    param[0] = new MySqlParameter
                    {
                        Value = Url,
                        ParameterName = "@validUrl"
                    };


                    DataSet dataset = Data.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, "usp_EL_Get_ExternalUrl", param);
                    if (dataset != null && dataset.Tables.Count > 0)
                    {
                        DataTable dataTable = dataset.Tables[0];
                        if (dataTable.Rows.Count > 1)
                        {
                            throw new Exception("More than one URL found. Cannot process");
                        }
                        else if (dataTable.Rows.Count == 1)
                        {
                            return dataTable;
                            //isUrlInList = true;
                        }
                        else
                        {
                            return dataTable;
                        }
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }

        }
    }
}
