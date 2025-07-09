import { useState } from "react";
import { path_createRegistroEncomienda, path_exportarEncomiendasPorRangoFechas, path_exportarTodasLasEncomiendas, path_getAllEncomiendas, path_retirarEncomienda } from "../../services/API/API-VPASS3";
import axios from "axios";
import usePersona from "../usePersona/usePersona";

const UseEncomienda = () => {

    const [loading, setLoading] = useState(false);
    const [response, setResponse] = useState(null);
    const [responseStatus, setResponseStatus] = useState(null);
    const [encomiendas, setEncomiendas] = useState(null);

    const {getPersonaByIdentificationNumber, crearPersona} = usePersona();

    const getAllEncomiendas = async () => {
        setLoading(true);
        try {
          const response = await axios.get(path_getAllEncomiendas);
          const status = response?.status || null;
          const responseData = response?.data || null;
          const encomiendasData = response?.data?.data || null;
          setResponse(responseData);
          setResponseStatus(status);
          setEncomiendas(encomiendasData);
          return responseData;
        } catch (error) {
          const status = error?.status ?? error?.statusCode ?? null;
          setResponse(error?.response?.data || error || "Error desconocido");
          setResponseStatus(status);
          return error?.response?.data || error || "Error desconocido";
        } finally {
          setLoading(false);
        }
    }

    const crearRegistroEncomienda = async (
        nombreDestinatario,
        codigoEncomienda,
        idDepartamento,
        nombrePersonaQueRetira,
        apellidoPersonaQueRetira,
        rutPersonaQueRetira,
        encomiendaFueRetirada,
        idInquilinoDepartamento
    ) => {
        setLoading(true);
        try {
            let personaRetira;
            let cuerpoPeticionCrearRegistroEncomienda;

            // Si la encomienda fue retirada, se obtiene o crea la persona que retira
            if(encomiendaFueRetirada){
                let respuestaObtenerPersona = await getPersonaByIdentificationNumber(rutPersonaQueRetira);

                const {data: dataObtenerPersona, statusCode: statusCodeObtenerPersona, message: messageObtenerPersona} = respuestaObtenerPersona;

                if(statusCodeObtenerPersona === 404){
                    const {data: dataCrearPersona} = await crearPersona({
                        nombres: nombrePersonaQueRetira,
                        apellidos: apellidoPersonaQueRetira,
                        numeroIdentificacion: rutPersonaQueRetira
                    })
                    personaRetira = dataCrearPersona;
                }

                else if(statusCodeObtenerPersona === 200){
                    personaRetira = dataObtenerPersona;
                }
                else{
                    // Si no se obtuvo un código 200 o 404, se considera un error inesperado
                    throw respuestaObtenerPersona;
                }

                cuerpoPeticionCrearRegistroEncomienda = {
                    recipient: nombreDestinatario,
                    code: codigoEncomienda,
                    idApartment: idDepartamento,
                    idApartmentOwnership: idInquilinoDepartamento,
                    idPersonWhoReceived: personaRetira.id,
                }
            }
            // Si la encomienda no fue retirada, no se registra la persona que retira
            else{
                cuerpoPeticionCrearRegistroEncomienda = {
                    recipient: nombreDestinatario,
                    code: codigoEncomienda,
                    idApartment: idDepartamento,
                    idApartmentOwnership: idInquilinoDepartamento,
                    idPersonWhoReceived: null, // No se registra persona que retira si la encomienda no fue retirada
                }
            }

            const {data: responseCrearRegistroEncomienda} = await axios.post(path_createRegistroEncomienda, cuerpoPeticionCrearRegistroEncomienda);
            const {data: registroCreado, statusCode: statusCrearRegistro, message: messageCrearRegistro} = responseCrearRegistroEncomienda;
            setResponse(registroCreado);
            setResponseStatus(statusCrearRegistro);
            return responseCrearRegistroEncomienda;

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

    // Funcion para enviar peticion al backend indicando que una encomienda ya fue retirada por su destinatario
    const retirarEncomienda = async (
        nombrePersonaQueRetira,
        apellidoPersonaQueRetira,
        rutPersonaQueRetira,
        idEncomienda,
    ) => {
        setLoading(true);
        try {
            let personaRetira;
            let cuerpoPeticionRetirarEncomienda;


            const respuestaObtenerPersona = await getPersonaByIdentificationNumber(rutPersonaQueRetira);
            const {data: dataObtenerPersona, statusCode: statusCodeObtenerPersona, message: messageObtenerPersona} = respuestaObtenerPersona;

            if(statusCodeObtenerPersona === 404){
                const {data: dataCrearPersona} = await crearPersona({
                    nombres: nombrePersonaQueRetira,
                    apellidos: apellidoPersonaQueRetira,
                    numeroIdentificacion: rutPersonaQueRetira
                })
                personaRetira = dataCrearPersona;
            }

            else if(statusCodeObtenerPersona === 200){
                personaRetira = dataObtenerPersona;
            }
            else{
                // Si no se obtuvo un código 200 o 404, se considera un error inesperado
                throw respuestaObtenerPersona;
            }

            cuerpoPeticionRetirarEncomienda = {
                idPackage: idEncomienda,
                idPersonWhoReceived: personaRetira.id
            }
            const {data: responseRetirarEncomienda} = await axios.put(path_retirarEncomienda, cuerpoPeticionRetirarEncomienda);
            const {data: registroEncomiendaActualizado, statusCode: statusRetirarEncomienda, message: messageRetirarEncomienda} = responseRetirarEncomienda;
            setResponse(registroEncomiendaActualizado);
            setResponseStatus(statusRetirarEncomienda);
            return responseRetirarEncomienda;
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

    // Funcion para solicitarle al servidor un excel con las encomiendas del usuario/establecimiento por rango de fechas y descargarlo desde el navegador
    const exportarEncomiendasPorRangoDeFechas = async (fechaInicio, fechaFinal) => {
        setLoading(true);
        try {
            const dto = {
            StartDate: fechaInicio,
            EndDate: fechaFinal
            };

            const response = await axios.post(path_exportarEncomiendasPorRangoFechas, dto, {
            responseType: 'blob'
            });

            // Crear URL temporal del blob
            const url = window.URL.createObjectURL(new Blob([response.data]));

            // Obtener nombre del archivo del header
            const contentDisposition = response.headers['content-disposition'];
            let fileName = 'encomiendas.xlsx';
            if (contentDisposition) {
            const fileNameMatch = contentDisposition.match(/filename="?(.+)"?/);
            if (fileNameMatch && fileNameMatch[1]) {
                fileName = fileNameMatch[1];
            }
            }

            // Crear link y forzar descarga
            const link = document.createElement('a');
            link.href = url;
            link.setAttribute('download', fileName);
            document.body.appendChild(link);
            link.click();

            document.body.removeChild(link);
            window.URL.revokeObjectURL(url);

            setResponse({ success: true });
            setResponseStatus(response.status);

            return { success: true, fileName };
        } catch (error) {
            let errorData;
            let status;

            if (error.response) {
            if (error.response.data instanceof Blob) {
                errorData = await error.response.data.text();
                try {
                errorData = JSON.parse(errorData);
                } catch {
                errorData = { message: errorData };
                }
            } else {
                errorData = error.response.data;
            }
            status = error.response.status;
            } else {
            errorData = error.message || 'Error desconocido';
            status = null;
            }

            setResponse(errorData);
            setResponseStatus(status);
            return { success: false, error: errorData };
        } finally {
            setLoading(false);
        }
    };

    // Funcion que le hace la peticion al backend para obtener todas las encomiendas del establecimiento y descargar el archivo desde el navegador
    const exportarTodasLasEncomiendas = async () => {
        setLoading(true);
        try {
            const response = await axios.get(path_exportarTodasLasEncomiendas, {
            responseType: 'blob'
            });

            // Crear URL temporal del blob
            const url = window.URL.createObjectURL(new Blob([response.data]));

            // Extraer nombre del archivo desde el header
            const contentDisposition = response.headers['content-disposition'];
            let fileName = 'encomiendas.xlsx';

            if (contentDisposition) {
            const fileNameMatch = contentDisposition.match(/filename="?(.+)"?/);
            if (fileNameMatch && fileNameMatch[1]) {
                fileName = fileNameMatch[1];
            }
            }

            // Crear link y forzar descarga
            const link = document.createElement('a');
            link.href = url;
            link.setAttribute('download', fileName);
            document.body.appendChild(link);
            link.click();

            document.body.removeChild(link);
            window.URL.revokeObjectURL(url);

            setResponse({ success: true });
            setResponseStatus(response.status);

            return { success: true, fileName };
        } catch (error) {
            let errorData;
            let status;

            if (error.response) {
            if (error.response.data instanceof Blob) {
                errorData = await error.response.data.text();
                try {
                errorData = JSON.parse(errorData);
                } catch {
                errorData = { message: errorData };
                }
            } else {
                errorData = error.response.data;
            }
            status = error.response.status;
            } else {
            errorData = error.message || 'Error desconocido';
            status = null;
            }

            setResponse(errorData);
            setResponseStatus(status);
            return { success: false, error: errorData };
        } finally {
            setLoading(false);
        }
};



    return ({
        loading,
        response,
        responseStatus,
        getAllEncomiendas,
        encomiendas,
        crearRegistroEncomienda,
        retirarEncomienda,
        exportarEncomiendasPorRangoDeFechas,
        exportarTodasLasEncomiendas
    });
}

export default UseEncomienda;