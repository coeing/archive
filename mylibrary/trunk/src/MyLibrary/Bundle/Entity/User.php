<?php

namespace MyLibrary\Bundle\Entity;

/**
 * 
 * User model
 * @author Christian Oeing
 *
 * @orm:Entity 
 */
class User
{
    /**
     * @orm:Id
     * @orm:Column(type="integer")
     * @orm:GeneratedValue(strategy="AUTO")
     */
	private $id;

    /**
     * @orm:Column(type="string", length="255")
     */
	private $name;
	
	/**
     * @orm:Column(type="string", length="255")
     */
	private $password;

	private $items;
    
    public function Name($name = null)
    {
        if ( $name === null )
        {
            return $this->name;
        }
        
        $this->name = $name;
    }
    
    public function Password($password = null)
    {
        if ($password === null)
        {
            return $this->password;
        }
        
        $this->password = $password;
    }
}