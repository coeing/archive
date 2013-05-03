<?php

require_once __DIR__.'/IAmazonPAResponse.php';

class AmazonPAResponseREST implements IAmazonPAResponse
{
	private $response;

	private $parsedXml;

	public function __construct($response)
	{
		$this->response = $response;
	}

	private function GetParsedXml()
	{
		if ( $this->parsedXml == null )
		{
			$this->parsedXml = simplexml_load_string($this->response);
		}

		return $this->parsedXml;
	}

	public function GetItems()
	{
		$parsedXml = $this->GetParsedXml();

		$items = array();
		foreach($parsedXml->Items->Item as $itemNode)
		{
			$item = new AmazonPAItem();
			
			$item->ASIN = $itemNode->ASIN;
			$item->Title = $itemNode->ItemAttributes->Title;
			
			array_push($items, $item);
		}
		
		return $items;
	}
}
