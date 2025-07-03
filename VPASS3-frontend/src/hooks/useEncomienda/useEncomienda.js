import { useState } from "react";
import { path_createRegistroEncomienda, path_getAllEncomiendas } from "../../services/API/API-VPASS3";
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



    return ({
        loading,
        response,
        responseStatus,
        getAllEncomiendas,
        encomiendas,
        crearRegistroEncomienda
    });
}

export default UseEncomienda;