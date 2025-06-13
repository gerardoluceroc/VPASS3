import { useEffect, useState } from "react";
import useZonas from "../../../hooks/useZonas/useZonas";
import "./GestionZonasPageComponent.css"
import { Accordion, AccordionDetails, AccordionSummary, Box, Fade, IconButton, Typography } from "@mui/material";
import ButtonTypeOne from "../../Buttons/ButtonTypeOne/ButtonTypeOne";
import DatagridResponsive from "../../Datagrid/DatagridResponsive/DatagridResponsive";
import TableSkeleton from "../../Skeleton/TableSkeleton/TableSkeleton";
import ExpandMoreIcon from '@mui/icons-material/ExpandMore';
import { IconoBorrar } from "../../IconButtons/IconButtons";
import { useConfirmDialog } from "../../../hooks/useConfirmDialog/useConfirmDialog";
import ModalLoadingMasRespuesta from "../../Modal/ModalLoadingMasRespuesta/ModalLoadingMasRespuesta";
import useSubZona from "../../../hooks/useSubZona/useSubZona";
import { eliminarSubZonaFromRows, eliminarZonaFromRowsById } from "./funcionesGestionZonasPageComponent";
import ModalCrearZona from "../../Modal/ModalCrearZona/ModalCrearZona";
import AddCircleIcon from '@mui/icons-material/AddCircle';
import ModalCrearSubZona from "../../Modal/ModalCrearSubZona/ModalCrearSubZona";

const GestionZonasPageComponent = () => {

    const { zonas, getAllZonas, loading: loadingZonas, eliminarZona } = useZonas();
    const { loading: loadingSubZona, eliminarSubZona } = useSubZona();

    useEffect(() => {
        getAllZonas();
    }, [])

    // Estado en donde se guardarán los datos de las zonas, es con el objetivo de manipular el arreglo
    const [rowsZonas, setRowsZonas] = useState();

    // Estado para la zona seleccionada, se usa para el modal de crear subzona
    const [idZonaSeleccionada, setIdZonaSeleccionada] = useState(null);

    // En el momento en que carguen las zonas se hace una copia para rows.
    useEffect(() => {
        if (!Array.isArray(zonas)) return;
        setRowsZonas(zonas);
    }, [zonas]);

    /////////////// Acciones y estados para loading, confirmación y el modal de crear zona u subzona ///////////////////////////////
    const [openModalCrearZona, setOpenModalCrearZona] = useState(false);
    const handleOpenModalCrearZona = () => setOpenModalCrearZona(true);  
    const handleCloseModalCrearZona = () => setOpenModalCrearZona(false);

    const [openModalCrearSubZona, setOpenModalCrearSubZona] = useState(false);
    const handleOpenModalCrearSubZona = () => setOpenModalCrearSubZona(true);  
    const handleCloseModalCrearSubZona = () => setOpenModalCrearSubZona(false);

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

    // Función a ejecutar para cuando el usuario presione el boton de eliminar en una fila
    const handleBorrarSubZona = async (subZona) => {

        const {id: idSubZona} = subZona || {};

        const confirmed = await confirm({
            title: "Eliminar subzona",
            message: "¿Deseas eliminar esta subzona del establecimiento?"
        });

        if(confirmed){
            setOpenLoadingRespuesta(true);

            // Se realiza la peticion al servidor para actualizar las subzonas borrando la subzona seleccionada
            const {statusCode: statusBorrarZona, message: messageBorrarZona} = await eliminarSubZona(idSubZona);

            // Si el servidor responde con el Response dto que tiene configurado
            if(statusBorrarZona != null && statusBorrarZona != undefined){

                if (statusBorrarZona === 200 || statusBorrarZona === 201 && (statusBorrarZona != null && statusBorrarZona != undefined)) {
                    setOperacionExitosa(true);
                    setMessageLoadingRespuesta(messageBorrarZona);

                    // // Se actualizan las rows eliminando aquella subzona seleccionada
                    const updatedRows = eliminarSubZonaFromRows(rowsZonas, idSubZona);
                    setRowsZonas(updatedRows);
                }
                else if (statusBorrarZona === 500) {
                    //En caso de error 500, se muestra un mensaje de error genérico, en vez del mensaje de error del backend
                    setOperacionExitosa(false);
                    setMessageLoadingRespuesta("Error desconocido, por favor intente nuevamente más tarde.");
                }
                else{
                    //En caso de cualquier otro error, se muestra el mensaje de error del backend
                    setOperacionExitosa(false);
                    setMessageLoadingRespuesta(messageBorrarZona);
                }
            }
            else{
                //Esto es para los casos que el servidor no responda el ResponseDto tipico
                setOperacionExitosa(false);
                setMessageLoadingRespuesta("Error desconocido, por favor intente nuevamente más tarde.");
            }
        }
    }

    // Función a ejecutar para cuando el usuario presione el boton de eliminar una zona
    const handleBorrarZona = async (zona) => {

        const {id: idZona} = zona || {};

        const confirmed = await confirm({
            title: "Eliminar zona",
            message: "¿Deseas eliminar esta zona del establecimiento?"
        });

        if(confirmed){
            setOpenLoadingRespuesta(true);

            // Se realiza la peticion al servidor para actualizar las zonas borrando la zona seleccionada
            const {statusCode: statusBorrarZona, message: messageBorrarZona} = await eliminarZona(idZona);

            // Si el servidor responde con el Response dto que tiene configurado
            if(statusBorrarZona != null && statusBorrarZona != undefined){

                if (statusBorrarZona === 200 || statusBorrarZona === 201 && (statusBorrarZona != null && statusBorrarZona != undefined)) {
                    setOperacionExitosa(true);
                    setMessageLoadingRespuesta(messageBorrarZona);

                    // // Se actualizan las rows eliminando aquella zona seleccionada
                    const updatedRows = eliminarZonaFromRowsById(rowsZonas, idZona);
                    setRowsZonas(updatedRows);
                }
                else if (statusBorrarZona === 500) {
                    //En caso de error 500, se muestra un mensaje de error genérico, en vez del mensaje de error del backend
                    setOperacionExitosa(false);
                    setMessageLoadingRespuesta("Error desconocido, por favor intente nuevamente más tarde.");
                }
                else{
                    //En caso de cualquier otro error, se muestra el mensaje de error del backend
                    setOperacionExitosa(false);
                    setMessageLoadingRespuesta(messageBorrarZona);
                }
            }
            else{
                //Esto es para los casos que el servidor no responda el ResponseDto tipico
                setOperacionExitosa(false);
                setMessageLoadingRespuesta("Error desconocido, por favor intente nuevamente más tarde.");
            }
        }
    }
    //////////////////////////////////////////////////////////////////////////////////////////////////////
    
    // Información que irá en la tabla
    const columnsZonas = ["Nombre", "Subzonas", "Acciones"];

    const dataZonas = rowsZonas?.map((zona) => {
        const {name: nameZona = ""} = zona;
        const {zoneSections: subZonas} = zona || {};
        const  columnsSubZonas = ["Nombre", "Acciones"];
        const dataSubZonas = subZonas?.map((subZona) => {
            const {name: nameSubZona = ""} = subZona;
            return [
                `${nameSubZona}`.trim(),
                <Box>
                    <IconoBorrar tituloToolTip={`Eliminar ${nameSubZona}`} handleClick={()=>handleBorrarSubZona(subZona)}/>
                </Box>
            ]
        })

        return[
            `${nameZona}`.trim(),
            <Accordion id="AccordionSubZonas">
                <AccordionSummary
                    expandIcon={<ExpandMoreIcon />}
                    aria-controls="panel1-content"
                    id="panel1-header"
                >
                    <Box display="flex" alignItems="center" justifyContent="space-between" width="100%">
                    <Typography component="span">{`Subzonas de ${nameZona}`}</Typography>
                    <IconButton
                        onClick={(e) => {
                        e.stopPropagation(); // Previene que se expanda el accordion
                        handleOpenModalCrearSubZona(); // Abre el modal para crear una subzona
                        setIdZonaSeleccionada(zona.id); // Guarda el id de la zona seleccionada para crear una subzona
                        }}
                        onFocus={(e) => e.stopPropagation()} // También para evitar comportamiento extraño con teclado
                    >
                        <AddCircleIcon />
                    </IconButton>
                    </Box>
                </AccordionSummary>
                <AccordionDetails>
                    <DatagridResponsive title={null} searchButton={false} viewColumnsButton={false} columns={columnsSubZonas} data={dataSubZonas} selectableRows="none"/>
                </AccordionDetails>
            </Accordion>,

            <Box>
                <IconoBorrar tituloToolTip={`Elimnar ${nameZona}`} handleClick={()=>handleBorrarZona(zona)}/>
            </Box>
        ]
    })

  return (
    <Box id="ContainerGestionZonasPageComponent">
        <Box id="BotonCrearNuevaZona">
            <ButtonTypeOne
                defaultText="Crear nueva zona"
                handleClick={handleOpenModalCrearZona}
            />
        </Box>
        <Fade in={!(!Array.isArray(rowsZonas))} timeout={{ enter: 500, exit: 300 }} unmountOnExit>
            <Box id="BoxDatagridResponsiveZonas">
                <DatagridResponsive title="Zonas" columns={columnsZonas} data={dataZonas} selectableRows="none"/>
                {ConfirmDialogComponent}
                <ModalLoadingMasRespuesta
                    open={openLoadingRespuesta}
                    loading={loadingSubZona || loadingZonas}
                    message={messageLoadingRespuesta}
                    loadingMessage="Eliminando zona..."
                    successfulProcess={operacionExitosa}
                    accionPostCierre={accionPostCierreLoadingRespuesta}
                />
                <ModalCrearZona
                    open={openModalCrearZona}
                    onClose={handleCloseModalCrearZona}
                    setRows={setRowsZonas}
                />
                <ModalCrearSubZona
                    open={openModalCrearSubZona}
                    onClose={handleCloseModalCrearSubZona}
                    setRows={setRowsZonas}
                    idZona={idZonaSeleccionada}
                />
            </Box>
        </Fade>

        <Fade in={!Array.isArray(rowsZonas)} timeout={{ enter: 500, exit: 300 }} unmountOnExit>
            <div>
                <TableSkeleton columnCount={2} rowCount={7} />
            </div>
        </Fade>
    </Box>
  )
}

export default GestionZonasPageComponent;