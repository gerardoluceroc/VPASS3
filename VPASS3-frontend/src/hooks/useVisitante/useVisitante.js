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
          const responseData = response?.data || null;
          const visitantesData = response?.data?.data || null;
          setResponse(responseData);
          setResponseStatus(status);
          setVisitantes(visitantesData);
          return responseData;
        } catch (error) {
            const dataError = error?.response?.data || error || "Error desconocido";
            const status = error?.status ?? error?.statusCode ?? null;
            setResponse(dataError);
            setResponseStatus(status);
            return dataError;
        } finally {
            setLoading(false);
        }
    }

    const getVisitanteByIdentificationNumber = async (identificationNumber) => {
      setLoading(true);
      try {
        const response = await axios.get(path_getVisitanteByIdentificationNumber + identificationNumber);
        const status = response?.status || null;
        const responseData = response?.data || null;
        setResponse(responseData);
        setResponseStatus(status);
        setVisitantes(responseData);
        return responseData;
      } catch (error) {
          const dataError = error?.response?.data || error || "Error desconocido";
          const status = error?.status ?? error?.statusCode ?? null;
          setResponse(dataError);
          setResponseStatus(status);
          return dataError;
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
        const responseData = response?.data || null;
        const visitanteCreado = response?.data?.data || null;
        setResponse(responseData);
        setResponseStatus(status);
        setVisitantes(visitanteCreado);
        return responseData;
      } catch (error) {
          const dataError = error?.response?.data || error || "Error desconocido";
          const status = error?.status ?? error?.statusCode ?? null;
          setResponse(dataError);
          setResponseStatus(status);
          return dataError;
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