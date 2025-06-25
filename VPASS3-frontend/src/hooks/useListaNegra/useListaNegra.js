import { useState } from "react";
import { path_createListaNegra, path_deleteListaNegraPorIdPersona, path_getAllListaNegra, path_updateListaNegra } from "../../services/API/API-VPASS3";
import axios from "axios";
import usePersona from "../usePersona/usePersona";

const useListaNegra = () => {
    const [loading, setLoading] = useState(false);
    const [response, setResponse] = useState(null);
    const [listaNegra, setListaNegra] = useState(null);
    const [responseStatus, setResponseStatus] = useState(null);

    const {crearPersona, getPersonaByIdentificationNumber} = usePersona();
  
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
            let idPersona = null;
            const responseCrearPersona = await crearPersona({
                nombres,
                apellidos,
                numeroIdentificacion
            });
            
            // Primero se intenta crear una nueva persona
            const { data: personaCreada, statusCode: statusCrearPersona, message: messageCrearPersona } = responseCrearPersona;

            if (statusCrearPersona === 409) {
                // Si la persona ya existe (código 409), se debe buscar por número de identificación
                const {data: personaExistente} = await getPersonaByIdentificationNumber(numeroIdentificacion);

                // Se extrae el ID de la persona existente
                idPersona = personaExistente?.id;
        
            } else if (statusCrearPersona === 201 || statusCrearPersona === 200) {
                // Si la persona fue creada exitosamente, se obtiene el ID directamente de la respuesta
                idPersona = personaCreada?.id;
        
            } else {
                // Si no se obtuvo un código 201 o 409, se considera un error inesperado
                throw responseCrearPersona;
            }

            const {data: responseCrearListaNegra} = await axios.post(path_createListaNegra,
                {
                    idPerson: idPersona,
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
            idPerson: idPersona
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