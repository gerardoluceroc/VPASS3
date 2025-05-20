// src/components/AxiosInterceptorProvider.jsx
import { useEffect, useRef } from 'react';
import { useSelector } from 'react-redux';
import axios from 'axios';

export const AxiosInterceptorProvider = ({ children }) => {
  const token = useSelector(state => state.user.token);
  const interceptorRef = useRef(null);

  useEffect(() => {
    // Configurar el interceptor de solicitud
    interceptorRef.current = axios.interceptors.request.use(config => {
      // Usar siempre el token actual de Redux
      if (token) {
        config.headers.Authorization = `Bearer ${token}`;
      } else {
        delete config.headers.Authorization;
      }
      return config;
    });

    return () => {
      // Limpiar el interceptor al desmontar
      if (interceptorRef.current !== null) {
        axios.interceptors.request.eject(interceptorRef.current);
      }
    };
  }, [token]); // Se recrea solo cuando el token cambia

  return children;
};