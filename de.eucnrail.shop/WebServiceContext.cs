using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Xml;


namespace de.eucnrail.shop
{
   public class WebServiceContext
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public static string CONTENT_TYP_XML = "text/xml; charset=utf-8";
        public static string CONTENT_TYP_JSON = "application/json;charset=utf-8";
        public static string CONTENT_TYP_IMAGE = "image/jpg";
        public static string CONTENT_TYP_HTML = "application/x-www-form-urlencoded";
        public static string REQUEST_METHOD_GET = "GET";
        public static string REQUEST_METHOD_POST = "POST";
        public static string REQUEST_METHOD_PUT = "PUT";
        private RootShop shop;

        public WebServiceContext(RootShop shop)
        {
            this.shop = shop;
        }
        public string GetOriginId(string origin)
        {
            string url = shop.GetServiceUrlWithFilter("product_feature_values", "value", origin);
            WebRequest req = CreateWebRequest(REQUEST_METHOD_GET, url, CONTENT_TYP_XML, null);
            XmlDocument xml = GetXmlResponse(req);
            XmlNode node = xml.SelectSingleNode("//prestashop/product_feature_values/product_feature_value/id");
            string id;
            if (node == null)
            {
                log.Info(string.Format("GetOriginId: Origin '{0}' Not Found", origin));
                CreateOrigin(origin);
                id = GetOriginId(origin);
            }
            else
                id = node.InnerText;
            log.Debug("GetOriginId: id = " + id);
            return id;
        }
        public string GetProductIdByReferenceId(string reference)
        {
            string id = String.Empty;
            WebRequest request = CreateWebRequest(REQUEST_METHOD_GET, shop.GetServiceUrlWithFilter("products", "reference", reference), CONTENT_TYP_XML, null);
            XmlDocument xml = GetXmlResponse(request);
            XmlNodeList nodes = xml.SelectNodes("//prestashop/products/product/id");
            if (nodes.Count == 1)
                id = nodes[0].InnerText;
            return id;
        }
        public void CreateOrigin(string origin)
        {
            log.Info("CreateOrigin: enter...");
            XmlDocument doc = GetBlackTemplate("product_feature_values");
            doc.SelectSingleNode("//prestashop/product_feature_value/id_feature").AppendChild(doc.CreateCDataSection("6"));
            XmlNodeList nodes = doc.SelectNodes("//prestashop/product_feature_value/value/language");
            foreach (XmlNode node in nodes) {
                node.AppendChild(doc.CreateCDataSection(origin));
            }
            string content = doc.OuterXml;
            WebRequest req = CreateWebRequest("POST", shop.GetServiceUrl("product_feature_values"), CONTENT_TYP_XML, content);
            log.Debug(content);
            GetResponse(req);
            log.Info("CreateOrigin: Leave");
        }

        public string GetmanufacturerId(string manufacturer)
        {
            string id;
            WebRequest request = CreateWebRequest(REQUEST_METHOD_GET, shop.GetServiceUrlWithFilter("manufacturers", "name", manufacturer), CONTENT_TYP_XML, null);
            XmlDocument xml = GetXmlResponse(request);
            XmlNode node = xml.SelectSingleNode("//prestashop/manufacturers/manufacturer/id");            
            if (node == null)
            {
                log.Info(string.Format("GetOriginId: Manufacturer '{0}' Not Found", manufacturer));
                CreateManufacturer(manufacturer);
                id = GetmanufacturerId(manufacturer);
            }
            else
                id = node.InnerText;
            log.Debug("GetmanufacturerId: id = " + id);
            return id;
        }

        public string GetCategoryId(string category)
        {
            string id;
            WebRequest request = CreateWebRequest(REQUEST_METHOD_GET, shop.GetServiceUrlWithFilter("categories", "name", category), CONTENT_TYP_XML, null);
            XmlDocument xml = GetXmlResponse(request);
            XmlNode node = xml.SelectSingleNode("//prestashop/categories/category/id");
            if (node == null)
            {               
                log.Error(string.Format("GetCategoryId: Category '{0}' Not Found", category));
                throw new ArgumentNullException(category, string.Format("Category '{0}' Not Found, please verify, the category exists in target system!", category));
            }
            id = node.InnerText;
            log.Debug("GetCategoryId: id = " + id);
            return id;
        }


        public string GetLanguageCNId() {
            string id;
            WebRequest request = CreateWebRequest(REQUEST_METHOD_GET, shop.GetServiceUrlWithFilter("languages", "name", "中文 (Simplified Chinese)"), CONTENT_TYP_XML, null);
            XmlDocument xml = GetXmlResponse(request);
            XmlNode node = xml.SelectSingleNode("//prestashop/languages/language/id");
            if (node == null)
            {
                log.Error(string.Format("GetLanguageCNId: Language '{0}' Not Found", "CN"));
                throw new ArgumentNullException("language", string.Format("Category '{0}' Not Found, please verify, the category exists in target system!", "CN"));
            }
            id = node.InnerText;
            log.Debug("GetLanguageCNId: id = " + id);
            return id;
        }

        public void CreateManufacturer(string manufacturer)
        {
            log.Info("CreateManufacturer: enter...");
            XmlDocument doc = GetBlackTemplate("manufacturers");
            doc.SelectSingleNode("//prestashop/manufacturer/name").AppendChild(doc.CreateCDataSection(manufacturer));
            doc.SelectSingleNode("//prestashop/manufacturer/active").AppendChild(doc.CreateCDataSection("1"));
            string content = doc.OuterXml;
            WebRequest req = CreateWebRequest(REQUEST_METHOD_POST, shop.GetServiceUrl("manufacturers"), CONTENT_TYP_XML, content);
            GetResponse(req);
            log.Info("CreateManufacturer: Leave");
        }

        public void TransmitImage(string productId, string imageUrl)
        {
            log.Info("TransmitImage: The Image transfer enter...");
            log.Info("TransmitImage: productId = " + productId);
            log.Info("TransmitImage: imageUrl = " + imageUrl);
            Dictionary<string, string> postParameters = new Dictionary<string, string>();
            //postParameters.Add("image_url", Uri.EscapeUriString(imageUrl));

            postParameters.Add("image_url", imageUrl);
            postParameters.Add("action_name", "add_image");
            postParameters.Add("target_url", shop.GetProductImageUrl(productId));
            postParameters.Add("api_key", shop.Key);
            postParameters.Add("image_mine", CONTENT_TYP_IMAGE);
            string content = "";
            foreach (string key in postParameters.Keys)
            {
                content += HttpUtility.UrlEncode(key) + "="
                      + HttpUtility.UrlEncode(postParameters[key]) + "&";
            }
            WebRequest request = CreateWebRequest(REQUEST_METHOD_POST, shop.UploadImageUrl, CONTENT_TYP_HTML, content);
            List<ResponseMsg> msgs = GetJsonResponse(request);
            ResponseMsg responseMsg = msgs.Find(a => a.code == "0");
            if(responseMsg != null)
                throw new Exception(responseMsg.msg);
            log.Info("TransmitImage: Image transmitted successfully!!");
        }
        public void TransmitProduct(Product product)
        {
            log.Info("TransmitProduct: Product transfer enter...");

            string id = GetProductIdByReferenceId(product.Reference);
            if (!string.Empty.Equals(id))
                throw new ArgumentException("reference id", string.Format("The Reference id {0} exists in system!!", product.Reference));
            product.OriginId = GetOriginId(product.Origin);
            product.BrandId = GetmanufacturerId(product.Brand);
            product.DefaultCategoryNr = GetCategoryId(product.DefaultCategory);
            product.LanguageCNId = GetLanguageCNId();

            string[] ids = new string[product.Categories.Length];
            for (int i = 0 ; i<product.Categories.Length;  i++)
            {
               ids[i]= GetCategoryId(product.Categories[i]);
            }
            product.CategoryIds = ids;
            string content = product.ProductDataContent;
            WebRequest request = CreateWebRequest(REQUEST_METHOD_POST, shop.GetServiceUrl("products"), CONTENT_TYP_XML, content);
            GetResponse(request);
            log.Info(string.Format("TransmitProduct: Product '{0}' transmitted successfully!!", product.Name));
        }
        public void UpdateProductPrice(string id, string price) {
            log.Info("UpdateProductPrice: Product Updating enter...");
            string sId = GetProductIdByReferenceId(id);
            if (String.Empty.Equals(sId))
                throw new ArgumentException("reference id", string.Format("No product found by the reference id '{0}'", id));
            
            WebRequest request = CreateWebRequest(REQUEST_METHOD_GET, shop.GetProductServiceUrl(sId), CONTENT_TYP_XML, null);
            XmlDocument xml = GetXmlResponse(request);
            log.Debug(xml.OuterXml);
            xml.SelectSingleNode("//prestashop/product/price").InnerXml = xml.CreateCDataSection(price).OuterXml;
            XmlNode node = xml.SelectSingleNode("//prestashop/product/manufacturer_name");
            node.ParentNode.RemoveChild(node);
            node = xml.SelectSingleNode("//prestashop/product/quantity");
            node.ParentNode.RemoveChild(node);
            WebRequest req = CreateWebRequest(REQUEST_METHOD_PUT, shop.GetProductServiceUrl(sId), CONTENT_TYP_XML, xml.OuterXml);
            GetXmlResponse(req);
            log.Info("UpdateProductPrice: Product Updated successfully.");
        }


        
        public XmlDocument GetBlackTemplate(string sType)
        {
            string svcUrl = shop.GetSchemaUrl(sType);
            WebRequest request = CreateWebRequest(REQUEST_METHOD_GET, svcUrl, CONTENT_TYP_XML, null);
            return GetXmlResponse(request);
        }
        private String GetResponse(WebRequest request)
        {
            WebResponse response = request.GetResponse();
            string responseFromServer;            
            log.Info("GetResponse: " + ((HttpWebResponse)response).StatusDescription);
            using (Stream dataStream = response.GetResponseStream())
            {
                // Open the stream using a StreamReader for easy access.  
                StreamReader reader = new StreamReader(dataStream);
                // Read the content.  
                responseFromServer = reader.ReadToEnd();
            }
            response.Close();
            return responseFromServer;
        }

        private List<ResponseMsg> GetJsonResponse(WebRequest request) {
            string[] s = GetResponse(request).Split('\n');
            string sJson = s[s.Length - 1];
            List<ResponseMsg>  msg = new JavaScriptSerializer().Deserialize<List<ResponseMsg>>(sJson);
            return msg;
        }
        private XmlDocument GetXmlResponse(WebRequest request) {
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(GetResponse(request));
            return xml;
        }
        private WebRequest CreateWebRequest(string requestType, string url, string contentType, string content)
        {
            log.Info(string.Format("CreateWebRequest: method={0} url={1} ", requestType, url));
            WebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.ContentType = contentType;
            request.Method = requestType;
            if (!string.IsNullOrEmpty(content))
            {
                byte[] byteArray = Encoding.UTF8.GetBytes(content);
                request.ContentLength = byteArray.Length;
                // Get the request stream.  
                Stream dataStream = request.GetRequestStream();
                // Write the data to the request stream.  
                dataStream.Write(byteArray, 0, byteArray.Length);
                // Close the Stream object.  
                dataStream.Close();
            }
            return request;
        }
        
    }

    public class ResponseMsg
    {
        public string id { get; set; }
        public string msg { get; set; }
        public string code { get; set; }
    }
}
