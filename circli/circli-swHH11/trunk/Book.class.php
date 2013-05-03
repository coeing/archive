<?php

class Book
{
	var $author;
	var $title;
	var $image;
	
	function Book($author, $title, $image)
	{
		$this->author = $author;
		$this->title = $title;
		$this->image = $image;
	}
	
	function FullTitle()
	{
		return $this->author." - ".$this->title;
	}
}