import { Box } from "@mui/material"
import "./GestionEstacionamientoPageComponent.css"
import useEstacionamiento from "../../../hooks/useEstacionamiento/useEstacionamiento"
import { useEffect, useState } from "react";
import TableSkeleton from "../../Skeleton/TableSkeleton/TableSkeleton";
import DatagridResponsive from "../../Datagrid/DatagridResponsive/DatagridResponsive";
import SwitchMui from "../../Switch/SwitchMui/SwitchMui";
import { useConfirmDialog } from "../../../hooks/useConfirmDialog/useConfirmDialog";
import ModalLoadingMasRespuesta from "../../Modal/ModalLoadingMasRespuesta/ModalLoadingMasRespuesta";

const GestionEstacionamientoPageComponent = () => {

    // Se obtienen las funciones y estados a utilizar del hook
    const {estacionamientos, getAllEstacionamientos, actualizarEstacionamiento, loading: loadingEstacionamientos} = useEstacionamiento();
    useEffect(() => {
      getAllEstacionamientos();
    }, [])

    // Se invoca la función para consultarle al usuario si está seguro de la acción a realizar
    const { confirm, ConfirmDialogComponent } = useConfirmDialog();

    // Estados y funciones para manejar el componente ModalLoadingMasRespuesta
    const [openLoadingRespuesta, setOpenLoadingRespuesta] = useState(false);
    const [messageLoadingRespuesta, setMessageLoadingRespuesta] = useState('');
    const [operacionExitosa, setOperacionExitosa] = useState(false);
    const accionPostCierreLoadingRespuesta = () => {
        setOpenLoadingRespuesta(false);
        setMessageLoadingRespuesta('');
    }

    // Estado en donde se guardarán los datos de estacionamientos, es con el objetivo de manipular el arreglo
    const [rows, setRows] = useState([]);

    // En el momento en que carguen los estacionamientos, se hace una copia para rows.
    useEffect(() => {
        if (!Array.isArray(estacionamientos)) return;
        setRows(estacionamientos);
    }, [estacionamientos]);

    // Funcion a ejecutar cuando el usuario presione el switch
    const handleActualizarEstacionamiento = async (estacionamiento) => {

        const confirmed = await confirm({
            title: "Actualizar disponibilidad de estacionamiento",
            message: "¿Desea actualizar la disponibilidad de este estacionamiento?"
        });

        if(confirmed){
            setOpenLoadingRespuesta(true);
            const cuerpoPeticion = {
                ...estacionamiento,
                isAvailable: !estacionamiento.isAvailable
            }

            // Se realiza la peticion al servidor para actualizar la disponibilidad del estacionamiento
            const {statusCode: statusActualizarEstacionamiento, data: estacionamientoActualizado, message: messageActualizarEstacionamiento} = await actualizarEstacionamiento(estacionamiento.id, cuerpoPeticion)

            // En caso de ser exitoso, se actualiza las rows a partir del estacionamiento actualizado que responde el servidor, se reeemplaza este estacionamiento actualido del arreglo "rows"
            if (statusActualizarEstacionamiento === 200 || statusActualizarEstacionamiento === 201 && (statusActualizarEstacionamiento != null && statusActualizarEstacionamiento != undefined)) {
                setOperacionExitosa(true);
                setMessageLoadingRespuesta(messageActualizarEstacionamiento);
                const rowsActualizados = rows.map(estacionamiento =>
                    estacionamiento.id === estacionamientoActualizado.id
                        ? estacionamientoActualizado
                        : estacionamiento
                );
                setRows(rowsActualizados);
            }
            else if (statusActualizarEstacionamiento === 500) {
                //En caso de error 500, se muestra un mensaje de error genérico, en vez del mensaje de error del backend
                setOperacionExitosa(false);
                setMessageLoadingRespuesta("Error desconocido, por favor intente nuevamente más tarde.");
            }
            else{
                //En caso de cualquier otro error, se muestra el mensaje de error del backend
                setOperacionExitosa(false);
                setMessageLoadingRespuesta(messageActualizarEstacionamiento);
            }
        }
    }

    // Información que irá en la tabla
    const columns = ["Nombre", "Disponibilidad"];
    const data = rows?.map((estacionamiento) => {
        const {name = "", isAvailable = null} = estacionamiento;

        return[
            `${name}`.trim(),
                <SwitchMui
                helperText={isAvailable ? "Habilitado" : "Ocupado"}
                secondaryLabel=""
                primaryLabel=""
                checked={isAvailable}
                handleChange={()=>handleActualizarEstacionamiento(estacionamiento)}
                />
        ]
    })

    // Si no ha cargado la información, se muestar un skeleton mientras carga
    if (!Array.isArray(rows)) {
        return <TableSkeleton columnCount={2} rowCount={4} />;
    }
    
    return (
        <Box id="ContainerGestionEstacionamientoPageComponent">
            <DatagridResponsive title="Estacionamientos" columns={columns} data={data} selectableRows="none" downloadCsvButton={false} />
            {ConfirmDialogComponent}
            <ModalLoadingMasRespuesta
                open={openLoadingRespuesta}
                loading={loadingEstacionamientos}
                message={messageLoadingRespuesta}
                loadingMessage="Registrando visita..."
                successfulProcess={operacionExitosa}
                accionPostCierre={accionPostCierreLoadingRespuesta}
            />
        </Box>
    )
}
export default GestionEstacionamientoPageComponent;