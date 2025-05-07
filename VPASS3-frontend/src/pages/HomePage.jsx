import axios from "axios";
import { useEffect } from "react";
import { InterceptorRequest, InterceptorResponse } from "../services/API/Interceptor";
import DrawerResponsive from "../components/Drawer/DrawerResponsive/DrawerResponsive";
import HomePageComponent from "../components/PagesComponents/HomePageComponent/HomePageComponent";

const HomePage = () => {
  InterceptorResponse();
  InterceptorRequest();

  return (
    <DrawerResponsive>
      <HomePageComponent/>
    </DrawerResponsive> 
  )
}

export default HomePage;