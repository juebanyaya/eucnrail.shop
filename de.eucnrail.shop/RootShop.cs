using System;

namespace de.eucnrail.shop
{
    public class RootShop
    {
        public static string PRODUCT_TEMPLATE_File = "Config/ProductTemplate.xml";
        public static string FEATURE_ORIGIN_ID = "5";
        public static string FOOD_TAX_ID = "3";
        //public static string DEFAULT_TAX_ID = "64";

        public string Key { get; set; }
        public string RootUrl { get; set; }
        public string GetServiceUrl(string dataType)
        {
                if (string.IsNullOrWhiteSpace(Key))
                    throw new MissingFieldException("API Key");
                if (string.IsNullOrWhiteSpace(RootUrl))
                    throw new MissingFieldException("Root URL");
            return string.Format("{0}/api/{1}?ws_key={2}", RootUrl, dataType, Key);
        }
        public string UploadImageUrl
        {
            get
            {
                return RootUrl + "/UploadImage.php";
            }        
        }
        public string GetProductImageUrl(string id) {
            return RootUrl + "/api/images/products/" + id;
        }
        public string GetSchemaUrl(string dataType) {
            return string.Format("{0}/api/{1}?schema=blank&ws_key={2}", RootUrl, dataType, Key);
        }
        public string GetServiceUrlWithFilter(string dataType, string field, string val) {
            return string.Format("{0}/api/{1}?ws_key={2}&display=[id,{3}]&filter[{3}]=[{4}]", RootUrl, dataType, Key, field, val);
        }

        public string GetProductServiceUrl(string id) {
            return string.Format("{0}/api/products/{1}?ws_key={2}", RootUrl, id, Key); 
        }
    }
}
