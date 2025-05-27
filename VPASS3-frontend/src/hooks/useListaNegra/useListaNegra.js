import { useState } from "react";
import { path_createListaNegra, path_deleteListaNegraPorIdPersona, path_getAllListaNegra, path_updateListaNegra } from "../../services/API/API-VPASS3";
import axios from "axios";
import useVisitante from "../useVisitante/useVisitante";

const useListaNegra = () => {
    const [loading, setLoading] = useState(false);
    const [response, setResponse] = useState(null);
    const [listaNegra, setListaNegra] = useState(null);
    const [responseStatus, setResponseStatus] = useState(null);

    const {crearVisitante, getVisitanteByIdentificationNumber} = useVisitante();
  
    const getAllListaNegra = async () => {
        setLoading(true);
        try {
          const response = await axios.get(path_getAllListaNegra);
          const status = response?.status || null;
          const responseData = response?.data || null;
          const listaNegraData = response?.data?.data || null;
          setResponse(responseData);
          setResponseStatus(status);
          setListaNegra(listaNegraData);
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

    const actualizarListaNegra = async (id, listaNegra) => {
        setLoading(true);
        try {
            const response = await axios.put(path_updateListaNegra + "/" + id, listaNegra);
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

    const crearListaNegra = async ({
        nombres,
        apellidos,
        numeroIdentificacion,
        idEstablecimiento,
        motivo
    }) => {
        setLoading(true);
        try {
            let idVisitante = null;
            const responseCrearVisitante = await crearVisitante({
                nombres,
                apellidos,
                numeroIdentificacion
            });
            
            // Primero se intenta crear un nuevo visitante
            const { data: visitanteCreado, statusCode: statusCrearVisitante, message: messageCrearVisitante } = responseCrearVisitante;

            if (statusCrearVisitante === 409) {
                // Si el visitante ya existe (código 409), se debe buscar por número de identificación
                const {data: visitanteExistente} = await getVisitanteByIdentificationNumber(numeroIdentificacion);

                // Se extrae el ID del visitante existente
                idVisitante = visitanteExistente?.id;
        
            } else if (statusCrearVisitante === 201 || statusCrearVisitante === 200) {
                // Si el visitante fue creado exitosamente, se obtiene el ID directamente de la respuesta
                idVisitante = visitanteCreado?.id;
        
            } else {
                // Si no se obtuvo un código 201 o 409, se considera un error inesperado
                throw responseCrearVisitante;
            }

            const {data: responseCrearListaNegra} = await axios.post(path_createListaNegra,
                {
                    idVisitor: idVisitante,
                    idEstablishment: idEstablecimiento,
                    reason: motivo
                }
            )
            const status = responseCrearListaNegra?.statusCode || null;
            const data = responseCrearListaNegra || null;
            setResponse(responseCrearListaNegra);
            setResponseStatus(status);
            return data;
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



    const BorrarDeListaNegraPorIdPersona = async ({idPersona, idEstablecimiento}) => {
        setLoading(true);
        const cuerpoPeticion = {
            idEstablishment: idEstablecimiento,
            idVisitor: idPersona
        }
        try {
            const response = await axios.post(path_deleteListaNegraPorIdPersona, cuerpoPeticion);
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
  
    return {
      loading,
      response,
      responseStatus,
      getAllListaNegra,
      listaNegra,
      actualizarListaNegra,
      crearListaNegra,
      BorrarDeListaNegraPorIdPersona
    };
  }
  
  export default useListaNegra;