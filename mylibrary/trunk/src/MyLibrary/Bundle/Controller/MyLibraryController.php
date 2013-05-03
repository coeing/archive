<?php

namespace MyLibrary\Bundle\Controller;

require_once __DIR__.'/../ext/AmazonECS/lib/AmazonECS.class.php';

// Define Amazon Web Services keys
defined('AWS_API_KEY') or define('AWS_API_KEY', 'AKIAIEPETVJOI7YXP52A');
defined('AWS_API_SECRET_KEY') or define('AWS_API_SECRET_KEY', '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');

use Symfony\Bundle\FrameworkBundle\Controller\Controller;
use Symfony\Component\HttpFoundation\RedirectResponse;

use MyLibrary\Bundle\utils\amazon\pa\AmazonPAResponseSOAP;

class MyLibraryController extends Controller
{
	public function indexAction()
	{
        return $this->render('MyLibraryBundle:Add:index.html.twig', array( "items" => array(), "keywords" => "" ));
	}
	
	public function searchAction()
	{
        $keywords = $this->get('request')->get("keywords", "");
        
        // Get amazon product advertising interface
        $amazonPA = new \AmazonECS(AWS_API_KEY, AWS_API_SECRET_KEY, 'DE');
        $amazonPA->soapClassmap = array('Item' => "MyLibrary\Bundle\Utils\Amazon\PA\AmazonPAItemSOAP",
                                        'SmallImage' => "MyLibrary\Bundle\Utils\Amazon\PA\AmazonPAImageSOAP");

        // Search for keyword
        $result = $amazonPA->category("All")->responseGroup('Small,Images')->search($keywords);

        $response = new AmazonPAResponseSOAP($result);
        $items = $response->GetItems();

        return $this->render('MyLibraryBundle:Add:index.html.twig', array( "items" => $items, "keywords" => $keywords ));	
	}
}
