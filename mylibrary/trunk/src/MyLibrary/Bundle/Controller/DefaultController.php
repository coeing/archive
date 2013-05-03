<?php

namespace MyLibrary\Bundle\Controller;

use Symfony\Bundle\FrameworkBundle\Controller\Controller;

class DefaultController extends Controller
{
    public function indexAction()
    {
        return $this->render('MyLibraryBundle:Default:index.html.twig');
    }
    
    public function welcomeAction($user)
    {
        return $this->render('MyLibraryBundle:Default:welcome.html.twig', array("user" => $user));    	
    }
}
