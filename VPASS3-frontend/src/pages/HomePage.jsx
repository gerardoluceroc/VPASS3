import axios from "axios";
import { useEffect } from "react";
import { InterceptorRequest, InterceptorResponse } from "../services/API/Interceptor";
import HomeComponent from "../components/Home/HomeComponent";

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
    <HomeComponent/>
  )
}

export default HomePage;