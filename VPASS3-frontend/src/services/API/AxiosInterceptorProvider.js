// src/components/AxiosInterceptorProvider.jsx
import { useEffect, useRef } from 'react';
import { useSelector } from 'react-redux';
import axios from 'axios';

export const AxiosInterceptorProvider = ({ children }) => {
  const token = useSelector(state => state.user.token);
  const interceptorRef = useRef(null);

  console.log("AxiosInterceptorProvider - Render. Token actual del estado:", token);

  useEffect(() => {
    console.log("AxiosInterceptorProvider - useEffect ejecutado. Token:", token);

    // Limpiar el interceptor anterior si existe
    if (interceptorRef.current !== null) {
      console.log("AxiosInterceptorProvider - Limpiando interceptor anterior:", interceptorRef.current);
      axios.interceptors.request.eject(interceptorRef.current);
    }

    // Configurar el nuevo interceptor de solicitud
    interceptorRef.current = axios.interceptors.request.use(config => {
      console.log("--- Interceptor de Axios EJECUTADO ---");
      console.log("interceptorRef de axios ejecutado, token en el interceptor:", token);
      if (token) {
        console.log("he entrado al if token del interceptor de axios, configurando Authorization header.");
        config.headers.Authorization = `Bearer ${token}`;
      } else {
        console.log("he entrado al else del interceptor de axios, token nulo, eliminando Authorization header.");
        delete config.headers.Authorization;
      }
      return config;
    }, error => {
      console.error("Interceptor de Axios - Error en la solicitud:", error);
      return Promise.reject(error);
    });

    console.log("AxiosInterceptorProvider - Nuevo interceptor configurado:", interceptorRef.current);

    return () => {
      // Limpiar el interceptor al desmontar
      if (interceptorRef.current !== null) {
        console.log("AxiosInterceptorProvider - Función de limpieza (desmontaje/cambio de dependencia). Ejecting interceptor:", interceptorRef.current);
        axios.interceptors.request.eject(interceptorRef.current);
        interceptorRef.current = null; // Establecer a null para evitar re-eject
      }
    };
  }, [token]); // Se recrea solo cuando el token cambia

  return children;
};