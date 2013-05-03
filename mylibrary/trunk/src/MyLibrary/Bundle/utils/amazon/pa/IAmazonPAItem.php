<?php

namespace MyLibrary\Bundle\Utils\Amazon\PA
{
	interface IAmazonPAItem
	{
		public function ASIN();

		public function Title();
		
		public function DetailPageURL();

		public function SmallImage();
	}
}