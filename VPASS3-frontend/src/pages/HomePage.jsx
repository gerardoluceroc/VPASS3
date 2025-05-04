import axios from "axios";
import { useEffect } from "react";
import { InterceptorRequest, InterceptorResponse } from "../services/API/Interceptor";
import DrawerResponsive from "../components/Drawer/DrawerResponsive/DrawerResponsive";
import HomePageComponent from "../components/PagesComponents/HomePageComponent/HomePageComponent";

const HomePage = () => {
  InterceptorResponse();
  InterceptorRequest();

  useEffect(() => {
    axios.get('http://localhost:5113/User/all')
      .then(response => {
        // Manejar la respuesta
        console.log(response.data);
      })
      .catch(error => {
        // Manejar errores
        console.error('Error al obtener los datos:', error);
      });
  }, []);

  return (
    <DrawerResponsive>
      <HomePageComponent/>
    </DrawerResponsive> 
  )
}

export default HomePage;