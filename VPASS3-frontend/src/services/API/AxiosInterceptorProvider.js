// src/components/AxiosInterceptorProvider.jsx
import { useEffect, useRef } from 'react';
import { useSelector } from 'react-redux';
import axios from 'axios';

export const AxiosInterceptorProvider = ({ children }) => {
  const token = useSelector(state => state.user.token);
  const interceptorRef = useRef(null);

  useEffect(() => {

    // Limpiar el interceptor anterior si existe
    if (interceptorRef.current !== null) {
      axios.interceptors.request.eject(interceptorRef.current);
    }

    // Configurar el nuevo interceptor de solicitud
    interceptorRef.current = axios.interceptors.request.use(config => {

      if (token) {
        config.headers.Authorization = `Bearer ${token}`;
      } else {
        delete config.headers.Authorization;
      }
      return config;
    }, error => {
      return Promise.reject(error);
    });

    return () => {
      // Limpiar el interceptor al desmontar
      if (interceptorRef.current !== null) {
        axios.interceptors.request.eject(interceptorRef.current);
        interceptorRef.current = null; // Establecer a null para evitar re-eject
      }
    };
  }, [token]); // Se recrea solo cuando el token cambia

  return children;
};