package controllers;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

import javax.xml.parsers.DocumentBuilder;
import javax.xml.parsers.DocumentBuilderFactory;

import org.w3c.dom.Document;
import org.w3c.dom.Element;
import org.w3c.dom.Node;
import org.w3c.dom.NodeList;

import play.Logger;
import play.data.validation.Required;

import com.amazon.advertising.api.sample.ResponseItem;
import com.amazon.advertising.api.sample.SignedRequestsHelper;

public class SearchController extends BaseController {
    public static void index() {
	render();
    }

    /*
     * Your AWS Access Key ID, as taken from the AWS Your Account page.
     */
    private static final String AWS_ACCESS_KEY_ID = "AKIAIEPETVJOI7YXP52A";

    /*
     * Your AWS Secret Key corresponding to the above ID, as taken from the AWS Your Account page.
     */
    private static final String AWS_SECRET_KEY = "CBXHYq3Dpr2nQQwi1Lod+kfjcehqdSMMZJuO9ygb";

    /*
     * Use one of the following end-points, according to the region you are interested in:
     * 
     * US: ecs.amazonaws.com CA: ecs.amazonaws.ca UK: ecs.amazonaws.co.uk DE: ecs.amazonaws.de FR: ecs.amazonaws.fr JP:
     * ecs.amazonaws.jp
     */
    private static final String ENDPOINT = "ecs.amazonaws.de";

    /*
     * Utility function to fetch the response from the service and extract the title from the XML.
     */
    private static String fetchTitle(String requestUrl) {
	String title = null;
	try {
	    DocumentBuilderFactory dbf = DocumentBuilderFactory.newInstance();
	    DocumentBuilder db = dbf.newDocumentBuilder();
	    Document doc = db.parse(requestUrl);
	    Node titleNode = doc.getElementsByTagName("Title").item(0);
	    title = titleNode.getTextContent();
	} catch (Exception e) {
	    throw new RuntimeException(e);
	}
	return title;
    }

    private static ResponseItem createFromXml(Element itemNode) {
	ResponseItem item = new ResponseItem();

	Node asinNode = itemNode.getElementsByTagName("ASIN").item(0);
	item.id = asinNode != null ? asinNode.getTextContent() : null;

	Node urlNode = itemNode.getElementsByTagName("DetailPageURL").item(0);
	item.Url = urlNode != null ? urlNode.getTextContent() : null;

	Element itemAttributesNode = (Element) itemNode.getElementsByTagName("ItemAttributes").item(0);
	if (itemAttributesNode != null) {
	    Node titleNode = itemAttributesNode.getElementsByTagName("Title").item(0);
	    item.Title = titleNode != null ? titleNode.getTextContent() : "No title";
	    Node authorNode = itemAttributesNode.getElementsByTagName("Author").item(0);
	    item.Author = authorNode != null ? authorNode.getTextContent() : "No author";
	    Element imageNode = (Element) itemNode.getElementsByTagName("MediumImage").item(0);
	    if (imageNode != null) {
		Node imageUrlNode = imageNode.getElementsByTagName("URL").item(0);
		item.ImageUrl = imageUrlNode != null ? imageUrlNode.getTextContent() : null;
		Node imageWidthNode = imageNode.getElementsByTagName("Width").item(0);
		item.ImageWidth = imageWidthNode != null ? Integer.parseInt(imageWidthNode.getTextContent()) : 0;
		Node imageHeightNode = imageNode.getElementsByTagName("Height").item(0);
		item.ImageHeight = imageHeightNode != null ? Integer.parseInt(imageHeightNode.getTextContent()) : 0;
	    }
	}

	return item;
    }

    public static void search(@Required String search) {
	Logger.debug("Search request '%s'.", search);

	/*
	 * Set up the signed requests helper
	 */
	SignedRequestsHelper helper;
	try {
	    helper = SignedRequestsHelper.getInstance(ENDPOINT, AWS_ACCESS_KEY_ID, AWS_SECRET_KEY);
	} catch (Exception e) {
	    e.printStackTrace();
	    return;
	}

	String requestUrl = null;
	String title = null;

	/* The helper can sign requests in two forms - map form and string form */

	/*
	 * Here is an example in map form, where the request parameters are stored in a map.
	 */
	System.out.println("Map form example:");
	Map<String, String> params = new HashMap<String, String>();
	params.put("Service", "AWSECommerceService");
	params.put("Version", "2010-11-01");
	params.put("Operation", "ItemSearch");
	params.put("SearchIndex", "Books");
	params.put("Keywords", search);
	params.put("ResponseGroup", "Small,Images,AlternateVersions");

	requestUrl = helper.sign(params);
	System.out.println("Signed Request is");
	System.out.println(requestUrl);

	title = fetchTitle(requestUrl);
	System.out.println("Signed Title is \"" + title + "\"");
	System.out.println();

	// Get needed information out of response xml.
	List<ResponseItem> items = new ArrayList<ResponseItem>();
	try {
	    DocumentBuilderFactory dbf = DocumentBuilderFactory.newInstance();
	    DocumentBuilder db = dbf.newDocumentBuilder();
	    Document doc = db.parse(requestUrl);

	    Element itemsNode = (Element) doc.getElementsByTagName("Items").item(0);

	    NodeList itemNodes = itemsNode.getElementsByTagName("Item");
	    for (int i = 0; i < itemNodes.getLength(); ++i) {
		Element itemNode = (Element) itemNodes.item(i);
		ResponseItem item = createFromXml(itemNode);
		items.add(item);
	    }
	} catch (Exception e) {
	    throw new RuntimeException(e);
	}

	render(search, items, requestUrl);
    }

    public static String autocomplete(@Required String term) {
	Logger.debug("Autocomplete request %s.", term);

	return String.format("%s1 %s2, %s3", term, term, term);
    }
}
