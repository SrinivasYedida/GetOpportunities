﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace GetOpportunities
{
    class BL_Opportunities
    {
        public static string[] CheckVSTokenExpiry()
        {
            return DA_Opportunities.CheckVSTokenExpiry();
        }

        public static int InsertVSToken(string Token, string TokenExpiryTime, string TokenType)
        {
            return DA_Opportunities.InsertVSToken(Token, TokenExpiryTime, TokenType);
        }
        public static List<CdkDealers> GetDMSActiveDealers(int GROUP_ID, int DST_ID, string FILESUBTYPE)
        {
            return DA_Opportunities.GetDMSActiveDealers(GROUP_ID, DST_ID, FILESUBTYPE);
        }
        public static List<OpportunitiesDeltaAction> OpportunitiesDeltaAction(string FILETYPE, string FILESUBTYPE, int FILETYPEDURATION)
        {
            return DA_Opportunities.EmployeesDeltaAction(FILETYPE, FILESUBTYPE, FILETYPEDURATION);
        }
    }

    public class CdkDealers : ICreatable
    {
        public int DMS_ID { get; set; }
        public int DMS_ENDUSER_ID { get; set; }
        public string DMS_ENDUSER_TITLE { get; set; }
        public string ENDUSER_TIMEZONE { get; set; }
        public string ZONESPAN { get; set; }
        public DateTime ZONETIME { get; set; }
        public string DSTDP_SERVICE_URL { get; set; }
        public string DS_APIKEY { get; set; }
        public string DS_SECRETKEY { get; set; }
        public string DED_SUBSCRIPTIONID { get; set; }

        public void Create(SqlDataReader SqlReader)
        {
            try
            {
                DMS_ID = SqlReader[SqlReader.GetOrdinal("DMS_ID")] as Int32? ?? default(Int32);
                DMS_ENDUSER_ID = SqlReader[SqlReader.GetOrdinal("DMS_ENDUSER_ID")] as Int32? ?? default(Int32);
                DMS_ENDUSER_TITLE = SqlReader[SqlReader.GetOrdinal("DMS_ENDUSER_TITLE")] as string;
                ENDUSER_TIMEZONE = SqlReader[SqlReader.GetOrdinal("ENDUSER_TIMEZONE")] as string;
                ZONESPAN = SqlReader[SqlReader.GetOrdinal("ZONESPAN")] as string;
                ZONETIME = SqlReader[SqlReader.GetOrdinal("ZONETIME")] as DateTime? ?? default(DateTime);
                DSTDP_SERVICE_URL = SqlReader[SqlReader.GetOrdinal("DSTDP_SERVICE_URL")] as string;
                DS_APIKEY = SqlReader[SqlReader.GetOrdinal("DS_APIKEY")] as string;
                DS_SECRETKEY = SqlReader[SqlReader.GetOrdinal("DS_SECRETKEY")] as string;
                DED_SUBSCRIPTIONID = SqlReader[SqlReader.GetOrdinal("DED_SUBSCRIPTIONID")] as string;
            }
            catch { }
        }
    }
    public class OpportunitiesDeltaAction : ICreatable
    {
        public string FILETYPE { get; set; }
        public string FILESUBTYPE { get; set; }
        public int FILETYPEDURATION { get; set; }

        public void Create(SqlDataReader SqlReader)
        {
            try
            {
                FILETYPE = SqlReader[SqlReader.GetOrdinal("FILETYPE")] as string;
                FILESUBTYPE = SqlReader[SqlReader.GetOrdinal("FILESUBTYPE")] as string;
                FILETYPEDURATION = SqlReader[SqlReader.GetOrdinal("FILETYPEDURATION")] as Int32? ?? default(Int32);
            }
            catch (Exception ee)
            {

            }
        }
    }
    public class VSDealers : ICreatable
    {
        public int VSDealerId { get; set; }
        public string VSDealerName { get; set; }
        public string VS_PostalCode { get; set; }

        public void Create(SqlDataReader SqlReader)
        {
            try
            {
                VSDealerId = SqlReader[SqlReader.GetOrdinal("dms_vs_dealer_ID")] as Int32? ?? default(Int32);
                ///VSDealerName = SqlReader[SqlReader.GetOrdinal("dms_subDealer_Name")] as string;
                //VS_PostalCode = SqlReader[SqlReader.GetOrdinal("dms_subDealer_PostalCode")] as string;
                //VSDealerId = SqlReader[SqlReader.GetOrdinal("dms_AiQ_dealer_id")] as Int32? ?? default(Int32);
                //VSDealerName = SqlReader[SqlReader.GetOrdinal("AiQDealerName")] as string;
                //VS_PostalCode = SqlReader[SqlReader.GetOrdinal("dms_subDealer_PostalCode")] as string;
            }
            catch (Exception ee)
            {

            }
        }
    }


    public class CompanyPositions : ICreatable
    {
        public int CPOS_ID { get; set; }
        public string CPOS_NAME { get; set; }

        public void Create(SqlDataReader SqlReader)
        {
            try
            {
                CPOS_ID = SqlReader[SqlReader.GetOrdinal("CPOS_ID")] as Int32? ?? default(Int32);
                CPOS_NAME = SqlReader[SqlReader.GetOrdinal("CPOS_NAME")] as string;
            }
            catch (Exception ee)
            {

            }
        }
    }
}
