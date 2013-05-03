<?php

namespace MyLibrary\Bundle\Controller;

use Symfony\Bundle\FrameworkBundle\Controller\Controller;
use Symfony\Component\HttpFoundation\RedirectResponse;

use MyLibrary\Bundle\Entity\User;

class UserController extends Controller
{    
    public function createAction($userName, $userPassword)
    {        
    	$user = new User();
        $user->Name($userName);
        $user->Password($userPassword);

        $em = $this->get('doctrine.orm.entity_manager');
        $em->persist($user);
        $em->flush();

        return $this->render('MyLibraryBundle:User:create.html.twig', array( "items" => $items, "keywords" => $keywords ));   
    }
    
    public function loginAction()
    {        
        $user = new User();
        return $this->render('MyLibraryBundle:User:login.html.twig', array("user" => $user));   
    }
    
    public function loginCheckAction()
    {
        $userName = $this->get('request')->get('username', '');
        $userPassword = $this->get('request')->get('password', '');
        
        $em = $this->get('doctrine.orm.entity_manager');
        
        $user = $em->getRepository('MyLibraryBundle:User')->findOneByName($userName);

        $error = "";
        if ($user == null)
        {
        	$error = "User ".$userName." doesn't exist";
        	$user = new User();
        }
        else if ($user->Password() != $userPassword)
        {
        	$error = "Wrong password";
        }
        
        if ($error != "")
        {
            return $this->render('MyLibraryBundle:User:login.html.twig', array("user" => $user, "error" => $error));
        }
        else
        {
        	return $this->forward('MyLibraryBundle:Default:welcome', array("user" => $user));
        }   
    }
    
    public function registerAction()
    {
        $user = new User();
        return $this->render('MyLibraryBundle:User:register.html.twig', array("user" => $user));       	
    }
    
    public function registerCheckAction()
    {
        $userName = $this->get('request')->get('username', '');
        $userPassword = $this->get('request')->get('password', '');
        $userPasswordRepeat = $this->get('request')->get('password_repeat', '');
		        
		$user = new User();
		$user->Name($userName);
		$user->Password($userPassword);
        
        $em = $this->get('doctrine.orm.entity_manager');
        
        // Check user name
        $error = "";
        if ( $em->getRepository('MyLibraryBundle:User')->findOneByName($userName) != null)
        {
        	// User already exists
        	$error = "User ".$userName." already exists";
        }
        else if ( $userPassword != $userPasswordRepeat )
        {
        	// Password repeat differs
            $error = "Password repetition differs";
        }
        else 
        {        	
	        $em->persist($user);
	        $em->flush();        	
        }
        
        if ( $error != "" )
        {
            return $this->render('MyLibraryBundle:User:register.html.twig', array("user" => $user, "error" => $error));
        }
        else
        {
            return $this->forward('MyLibraryBundle:Default:welcome', array("user" => $user));      	
        }
    }
}
