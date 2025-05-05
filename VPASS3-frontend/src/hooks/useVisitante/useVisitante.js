import { useState } from "react";
import { path_createVisitante, path_getAllVisitantes, path_getVisitanteByIdentificationNumber } from "../../services/API/API-VPASS3";
import axios from "axios";

const useVisitante = () => {

    const [loading, setLoading] = useState(false);
    const [response, setResponse] = useState(null);
    const [visitantes, setVisitantes] = useState(null);
    const [responseStatus, setResponseStatus] = useState(null);

    const getAllVisitantes = async () => {
        setLoading(true);
        try {
          const response = await axios.get(path_getAllVisitantes);
          const status = response?.status || null;
          setResponse(response || null);
          setResponseStatus(status);
          setVisitantes(response?.data?.data || null);
        } catch (error) {
          const errorMessage = error?.response?.data?.message || "Error desconocido";
          const status = error?.response?.status || null;
          setResponse(errorMessage);
          setResponseStatus(status);
        } finally {
          setLoading(false);
        }
    }

    const getVisitanteByIdentificationNumber = async (identificationNumber) => {
      setLoading(true);
      try {
        const response = await axios.get(path_getVisitanteByIdentificationNumber + identificationNumber);
        const status = response?.status || null;
        setResponse(response || null);
        setResponseStatus(status);
        setVisitantes(response?.data?.data || null);
        return response?.data?.data || null;
      } catch (error) {
        const errorMessage = error?.response?.data?.message || "Error desconocido";
        const status = error?.response?.status || null;
        setResponse(errorMessage);
        setResponseStatus(status);
        return null;
      } finally {
        setLoading(false);
      }
  }

    const crearVisitante = async ({ nombres, apellidos, numeroIdentificacion }) => {
      const data = {
        names: nombres,
        lastnames: apellidos,
        identificationNumber: numeroIdentificacion
      };
      setLoading(true);
      try {
        const response = await axios.post(path_createVisitante, data);
        const status = response?.status || null;
        setResponse(response || null);
        setResponseStatus(status);
        setVisitantes(response?.data?.data || null);
        return { data: response?.data?.data || null, status };
      } catch (error) {
        const errorMessage = error?.response?.data?.message || "Error desconocido";
        const status = error?.response?.status || null;
        setResponse(errorMessage);
        setResponseStatus(status);
        return { data: null, status };
      } finally {
        setLoading(false);
      }
    };
  
    
    return {
        loading,
        response,
        responseStatus,
        visitantes,
        crearVisitante,
        getAllVisitantes,
        getVisitanteByIdentificationNumber
    };
};
export default useVisitante;