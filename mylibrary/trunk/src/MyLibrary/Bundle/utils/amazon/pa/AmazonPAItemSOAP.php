<?php

namespace MyLibrary\Bundle\Utils\Amazon\PA
{
	require_once __DIR__.'/IAmazonPAItem.php';

	class AmazonPAItemSOAP implements IAmazonPAItem
	{
		// Amazon Standard Identification Number
		private $ASIN;

		// URL to amazon detail page
		private $DetailPageURL;

		// Product group
		private $ProductGroup;

		// Title
		private $ItemAttributes;
		
		// Images
		private $SmallImage;
		private $MediumImage;
		private $LargeImage;

		public function ASIN()
		{
			return $this->ASIN;
		}

		public function DetailPageURL()
		{
			return $this->DetailPageURL;
		}
		
		public function SmallImage()
		{
			return $this->SmallImage;
		}

		public function MediumImage()
		{
			return $this->MediumImage;
		}

		public function LargeImage()
		{
			return $this->LargeImage;
		}

		public function ProductGroup()
		{
			return $this->ItemAttributes->ProductGroup;
		}

		public function Title($value = null)
		{
			if ($value === null)
			{
				return $this->ItemAttributes->Title;
			}

			$this->ItemAttributes->Title = $value;
		}

		static public function CreateFrom($response)
		{
			if ( is_array($response) )
			{
				return AmazonPAItem::CreateFromArray($response);
			}
			else
			{
				return AmazonPAItem::CreateFromObject($response);
			}
		}

		static public function CreateFromArray($response)
		{
			$item = new AmazonPAItem();
			$item->ASIN = $response["ASIN"];
			$item->Title = $response["Title"];
			$item->ProductGroup = $response["ProductGroup"];
			$item->DetailPageURL = $response["DetailPageURL"];

			return $item;
		}

		static public function CreateFromObject($response)
		{
			$item = new AmazonPAItem();

			// TODO: Read contents

			return $item;
		}

		public function ToString()
		{
			return sprintf("ASIN: $this->ASIN, Title: ".$this->Title()."");//, Product Group: $this->ProductGroup, URL: $this->DetailPageURL");
		}
	}
}