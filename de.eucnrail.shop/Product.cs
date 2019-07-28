using System.Linq;
using System.Xml;

namespace de.eucnrail.shop
{
    public class Product
    {
        public string Origin { get; set; }
        public string Brand { get; set; }
        public string BrandId { get; set; }
        public string OriginId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Price { get; set; }
        public string Reference { get; set; }
        public string DefaultCategoryNr { get; set; }
        public string DefaultCategory { get; set; }
        public string[] Categories { get; set; }
        public string[] CategoryIds { get; set; }
        public string TaxId { get; set; }

        public string ProductDataContent
        {
            get
            {
                XmlDocument xml = new XmlDocument();
                xml.Load(RootShop.PRODUCT_TEMPLATE_File);
                xml.SelectSingleNode("//prestashop/product/id_manufacturer").AppendChild(xml.CreateCDataSection(BrandId));
                xml.SelectSingleNode("//prestashop/product/id_category_default").AppendChild(xml.CreateCDataSection(DefaultCategoryNr));
                //xml.SelectSingleNode("//prestashop/product/name/language").AppendChild(xml.CreateCDataSection(Name));
                //xml.SelectSingleNode("//prestashop/product/description/language").AppendChild(xml.CreateCDataSection(Description));
                xml.SelectSingleNode("//prestashop/product/price").AppendChild(xml.CreateCDataSection(Price));
                xml.SelectSingleNode("//prestashop/product/reference").AppendChild(xml.CreateCDataSection(Reference));
                xml.SelectSingleNode("//prestashop/product/associations/product_features/product_feature/id").AppendChild(xml.CreateCDataSection(RootShop.FEATURE_ORIGIN_ID));
                xml.SelectSingleNode("//prestashop/product/associations/product_features/product_feature/id_feature_value").AppendChild(xml.CreateCDataSection(OriginId));
                xml.SelectSingleNode("//prestashop/product/id_tax_rules_group").AppendChild(xml.CreateCDataSection(TaxId));

                XmlNode rewriteNode = xml.SelectSingleNode("//prestashop/product/link_rewrite");
                XmlNode languageDefault = rewriteNode.AppendChild(xml.CreateNode(XmlNodeType.Element, "language", null));
                XmlAttribute attribut = xml.CreateAttribute("id");
                attribut.Value = "1";
                languageDefault.Attributes.Append(attribut);
                languageDefault.AppendChild(xml.CreateCDataSection(Reference));

                XmlNode languageCN = rewriteNode.AppendChild(xml.CreateNode(XmlNodeType.Element, "language", null));
                attribut = xml.CreateAttribute("id");
                attribut.Value = LanguageCNId;
                languageCN.Attributes.Append(attribut);
                languageCN.AppendChild(xml.CreateCDataSection(Reference));


                XmlNode productNameNode = xml.SelectSingleNode("//prestashop/product/name");
                languageDefault = productNameNode.AppendChild(xml.CreateNode(XmlNodeType.Element, "language", null));
                attribut = xml.CreateAttribute("id");
                attribut.Value = "1";
                languageDefault.Attributes.Append(attribut);
                languageDefault.AppendChild(xml.CreateCDataSection(Name));

                languageCN= productNameNode.AppendChild(xml.CreateNode(XmlNodeType.Element, "language", null));
                attribut = xml.CreateAttribute("id");
                attribut.Value = LanguageCNId;
                languageCN.Attributes.Append(attribut);
                languageCN.AppendChild(xml.CreateCDataSection(Name));

                foreach (string s in CategoryIds.Concat(new string[] { DefaultCategoryNr }))
                {
                    XmlNode categoryNode = xml.CreateNode(XmlNodeType.Element, "category", null);
                    XmlNode idNode = xml.CreateNode(XmlNodeType.Element, "id", null);
                    idNode.AppendChild(xml.CreateCDataSection(s));
                    categoryNode.AppendChild(idNode);
                    xml.SelectSingleNode("//prestashop/product/associations/categories").AppendChild(categoryNode);
                }
                return xml.OuterXml;
            }
        }

        public string LanguageCNId { get; internal set; }
    }
}
