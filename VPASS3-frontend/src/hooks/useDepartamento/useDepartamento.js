import axios from "axios";
import { useState } from "react";
import { path_createDepartamento, path_deleteDepartamento, path_getAllDepartamentos } from "../../services/API/API-VPASS3";

const useDepartamento = () => {

    const [loading, setLoading] = useState(false);
    const [response, setResponse] = useState(null);
    const [responseStatus, setResponseStatus] = useState(null);
    const [departamentos, setDepartamentos] = useState(null);

    const getAllDepartamentos = async () => {
        setLoading(true);
        try {
          const response = await axios.get(path_getAllDepartamentos);
          const status = response?.status || null;
          const responseData = response?.data || null;
          const departamentosData = response?.data?.data || null;
          setResponse(responseData);
          setResponseStatus(status);
          setDepartamentos(departamentosData);
          return responseData;
        } catch (error) {
          const dataError = error?.response?.data || error || "Error desconocido";
          const status = error?.status ?? error?.statusCode ?? null;
          setResponse(dataError);
          setResponseStatus(status);
          return error?.response?.data || error;
        } finally {
          setLoading(false);
        }
    }
    
    const eliminarDepartamento = async (id) => {
        setLoading(true);
        try {
            const response = await axios.delete(path_deleteDepartamento + "/" + id);
            const status = response?.status || null;
            const responseData = response?.data || null;
            setResponse(responseData);
            setResponseStatus(status);
            return responseData;
          } catch (error) {
            const errorMessage = error?.response?.data?.message || "Error desconocido";
            const status = error?.response?.status || null;
            setResponse(errorMessage);
            setResponseStatus(status);
            return error;
          } finally {
            setLoading(false);
          }
    }

    const crearDepartamento = async (idZona, nombreDepartamento) => {
      setLoading(true);
      try {
        const {data: responseCrearDepartamento} = await axios.post(path_createDepartamento, {
          name: nombreDepartamento,
          idZone: idZona
        });

        // Primero se intenta crear un nuevo departamento
        const { data: departamentoCreado, statusCode: statusCrearDepartamento, message: messageCrearDepartamento } = responseCrearDepartamento;
        setResponse(departamentoCreado);
        setResponseStatus(statusCrearDepartamento);
        return responseCrearDepartamento;

      } catch (error) {
          const errorMessage = error?.response?.data?.message || "Error desconocido";
          const status = error?.response?.status || null;
          setResponse(errorMessage);
          setResponseStatus(status);
          return error;
      } finally {
        setLoading(false);
      }
    }

    return {
        loading,
        response,
        responseStatus,
        eliminarDepartamento,
        crearDepartamento,
        departamentos,
        getAllDepartamentos
    };
};
export default useDepartamento;