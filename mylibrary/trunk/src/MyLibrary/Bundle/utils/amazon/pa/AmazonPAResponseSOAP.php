<?php

namespace MyLibrary\Bundle\Utils\Amazon\PA
{
	require_once __DIR__.'/IAmazonPAResponse.php';

	class AmazonPAResponseSOAP implements IAmazonPAResponse
	{
		private $response;

		public function __construct($response)
		{
			$this->response = $response;
		}

		public function GetItems()
		{
			if ( $this->response != null &&
			$this->response->Items != null &&
			isset($this->response->Items->Item) )
			{
				return $this->response->Items->Item;
			}
			else
			{
				return array();
			}
		}
	}
}
