<?php

namespace MyLibrary\Bundle\Utils\Amazon\PA
{
	require_once __DIR__.'/IImage.php';
	
	class ImageSOAP implements IImage
	{
		private $URL;
		private $Height;
		private $Width;
		
		public function URL()
		{
			return $this->URL;
		}
		
		public function Height()
		{
			return $this->Height->_;
		}
		
		public function Width()
		{
			return $this->Width->_;
		}
	}
}