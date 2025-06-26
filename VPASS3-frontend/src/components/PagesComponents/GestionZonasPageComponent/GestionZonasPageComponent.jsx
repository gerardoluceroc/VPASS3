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
import { eliminarDepartamentoFromRows, eliminarZonaFromRowsById } from "./funcionesGestionZonasPageComponent";
import ModalCrearZona from "../../Modal/ModalCrearZona/ModalCrearZona";
import AddCircleIcon from '@mui/icons-material/AddCircle';
import useDepartamento from "../../../hooks/useDepartamento/useDepartamento";
import ModalCrearDepartamento from "../../Modal/ModalCrearSubZona/ModalCrearDepartamento";
import TooltipTipoUno from "../../Tooltip/TooltipTipoUno/TooltipTipoUno";

const GestionZonasPageComponent = () => {

    const { zonas, getAllZonas, loading: loadingZonas, eliminarZona } = useZonas();
    const { loading: loadingDepartamento, eliminarDepartamento } = useDepartamento();

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

    /////////////// Acciones y estados para loading, confirmación y el modal de crear zona o departamento ///////////////////////////////
    const [openModalCrearZona, setOpenModalCrearZona] = useState(false);
    const handleOpenModalCrearZona = () => setOpenModalCrearZona(true);  
    const handleCloseModalCrearZona = () => setOpenModalCrearZona(false);

    const [openModalCrearDepartamento, setOpenModalCrearDepartamento] = useState(false);
    const handleOpenModalCrearDepartamento = () => setOpenModalCrearDepartamento(true);  
    const handleCloseModalCrearDepartamento = () => setOpenModalCrearDepartamento(false);

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
    const handleBorrarDepartamento = async (departamento) => {

        const {id: idDepartamento} = departamento || {};

        const confirmed = await confirm({
            title: "Eliminar departamento",
            message: "¿Deseas eliminar este departamento del establecimiento?"
        });

        if(confirmed){
            setOpenLoadingRespuesta(true);

            // Se realiza la peticion al servidor para actualizar las zonas borrando el departamento seleccionado
            const {statusCode: statusBorrarDepartamento, message: messageBorrarDepartamento} = await eliminarDepartamento(idDepartamento);

            // Si el servidor responde con el Response dto que tiene configurado
            if(statusBorrarDepartamento != null && statusBorrarDepartamento != undefined){

                if (statusBorrarDepartamento === 200 || statusBorrarDepartamento === 201 && (statusBorrarDepartamento != null && statusBorrarDepartamento != undefined)) {
                    setOperacionExitosa(true);
                    setMessageLoadingRespuesta(messageBorrarDepartamento);

                    // // Se actualizan las rows eliminando aquel departamento seleccionado
                    const updatedRows = eliminarDepartamentoFromRows(rowsZonas, idDepartamento);
                    setRowsZonas(updatedRows);
                }
                else if (statusBorrarDepartamento === 500) {
                    //En caso de error 500, se muestra un mensaje de error genérico, en vez del mensaje de error del backend
                    setOperacionExitosa(false);
                    setMessageLoadingRespuesta("Error desconocido, por favor intente nuevamente más tarde.");
                }
                else{
                    //En caso de cualquier otro error, se muestra el mensaje de error del backend
                    setOperacionExitosa(false);
                    setMessageLoadingRespuesta(messageBorrarDepartamento);
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
            message: "¿Deseas eliminar esta zona del establecimiento?, también se borrarán todos sus departamentos asociados."
        });

        if(confirmed){
            setOpenLoadingRespuesta(true);

            // Se realiza la peticion al servidor para actualizar las zonas borrando la zona seleccionada
            const {statusCode: statusBorrarDepartamento, message: messageBorrarDepartamento} = await eliminarZona(idZona);

            // Si el servidor responde con el Response dto que tiene configurado
            if(statusBorrarDepartamento != null && statusBorrarDepartamento != undefined){

                if (statusBorrarDepartamento === 200 || statusBorrarDepartamento === 201 && (statusBorrarDepartamento != null && statusBorrarDepartamento != undefined)) {
                    setOperacionExitosa(true);
                    setMessageLoadingRespuesta(messageBorrarDepartamento);

                    // // Se actualizan las rows eliminando aquella zona seleccionada
                    const updatedRows = eliminarZonaFromRowsById(rowsZonas, idZona);
                    setRowsZonas(updatedRows);
                }
                else if (statusBorrarDepartamento === 500) {
                    //En caso de error 500, se muestra un mensaje de error genérico, en vez del mensaje de error del backend
                    setOperacionExitosa(false);
                    setMessageLoadingRespuesta("Error desconocido, por favor intente nuevamente más tarde.");
                }
                else{
                    //En caso de cualquier otro error, se muestra el mensaje de error del backend
                    setOperacionExitosa(false);
                    setMessageLoadingRespuesta(messageBorrarDepartamento);
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
    const columnsZonas = ["Nombre", "Departamentos", "Acciones"];

    const dataZonas = rowsZonas?.map((zona) => {
        const {name: nameZona = ""} = zona;
        const {apartments: departamentos} = zona || {};
        const  columnsDepartamentos = ["Nombre", "Acciones"];
        const dataDepartamentos = departamentos?.map((departamento) => {
            const {name: nameDepartamento = ""} = departamento;
            return [
                `${nameDepartamento}`.trim(),
                <Box>
                    <IconoBorrar tituloToolTip={`Eliminar ${nameDepartamento}`} handleClick={()=>handleBorrarDepartamento(departamento)}/>
                </Box>
            ]
        })

        return[
            `${nameZona}`.trim(),
            <Accordion id="AccordionDepartamentos">
                <Box display="flex" alignItems="center" justifyContent="space-between" width="100%">
                    <AccordionSummary
                        expandIcon={<ExpandMoreIcon />}
                        disableRipple
                        disableTouchRipple
                        sx={{ flexGrow: 1 }}
                    >
                        <Typography component="span">{`Departamentos en ${nameZona}`}</Typography>
                    </AccordionSummary>
                    
                    <TooltipTipoUno titulo={`Crear nuevo departamento en ${nameZona}`}>
                        <IconButton
                            onClick={(e) => {
                            e.stopPropagation(); // evita expandir
                            handleOpenModalCrearDepartamento();
                            setIdZonaSeleccionada(zona.id);
                            }}
                            onFocus={(e) => e.stopPropagation()}
                            size="small"
                        >
                            <AddCircleIcon />
                        </IconButton>
                    </TooltipTipoUno>
                </Box>
                <AccordionDetails>
                    <DatagridResponsive title={null} searchButton={false} viewColumnsButton={false} columns={columnsDepartamentos} data={dataDepartamentos} selectableRows="none"/>
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
                    loading={loadingDepartamento || loadingZonas}
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
                <ModalCrearDepartamento
                    open={openModalCrearDepartamento}
                    onClose={handleCloseModalCrearDepartamento}
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