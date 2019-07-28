using System;

namespace de.eucnrail.shop
{
    class Program
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        static void Main(string[] args)
        {
            try
            {
                Program test = new Program();
                RootShop rootShop = new RootShop();
                Product product = new Product();

                rootShop.RootUrl = "https://eucnrail.de/tao";
                rootShop.Key = "3F6H9C9I8XEUQ3HGSEWPYWCEUF7G6DGE";

                product.Name = "爱他美婴幼儿奶粉";
                product.Description = "爱他美婴幼儿奶粉 Pre段(0-6个月)800g";
                product.Price = "17.99";
                product.Reference = "0001";
                product.DefaultCategory = "主页";
                product.Categories = new string[] {"母婴玩具", "食品保健"};
                product.Origin = "德国";
                product.Brand = "Aptamil";
                product.TaxId = RootShop.FOOD_TAX_ID; //RootShop.DEFAULT_TAX_ID
                
                string url = "http://39.97.167.36:8006/data/upload/shop/store/goods/5/5_05717788550858139_360.jpg";
                test.CreateProduct(product, rootShop);
                
                test.UploadImage(product.Reference, url, rootShop);
                //test.UpdateProductPrice(product.Reference, rootShop);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        public void UpdateProductPrice(string refId, RootShop rootShop) {
            WebServiceContext webserviceContext = new WebServiceContext(rootShop);
            webserviceContext.UpdateProductPrice(refId, "6.9");
        }
        public void UploadImage(string refId,  string url, RootShop rootShop) {
            log.Info("============================== IMAGE TRANSFER START=================================");
            WebServiceContext webserviceContext = new WebServiceContext(rootShop);
            string id = webserviceContext.GetProductIdByReferenceId(refId);
            webserviceContext.TransmitImage(id, url);
            log.Info("=============================== IMAGE TRANSFER END ==================================");
        }
        public void CreateProduct(Product product, RootShop rootShop) {
            log.Info("============================== PRODUCT TRANSFER START ==============================");
            WebServiceContext webserviceContext = new WebServiceContext(rootShop);
                webserviceContext.TransmitProduct(product);
            log.Info("=============================== PRODUCT TRANSFER END ===============================");

        }
    }
}
