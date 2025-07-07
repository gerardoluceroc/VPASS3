import { use, useEffect, useState } from "react";
import "./GestionEncomiendasPageComponent.css";
import UseEncomienda from "../../../hooks/useEncomienda/useEncomienda";
import useDepartamento from "../../../hooks/useDepartamento/useDepartamento";
import { cambiarFormatoHoraFecha } from "../../../utils/funciones";
import { Box, Chip, Fade, IconButton } from "@mui/material";
import ButtonTypeOne from "../../Buttons/ButtonTypeOne/ButtonTypeOne";
import DatagridResponsive from "../../Datagrid/DatagridResponsive/DatagridResponsive";
import TableSkeleton from "../../Skeleton/TableSkeleton/TableSkeleton";
import InfoIcon from '@mui/icons-material/Info';
import TooltipTipoUno from "../../Tooltip/TooltipTipoUno/TooltipTipoUno";
import dayjs from "dayjs";
import ModalVerDetallesEncomienda from "../../Modal/ModalVerDetallesEncomienda/ModalVerDetallesEncomienda";
import ModalRegistarEncomienda from "../../Modal/ModalRegistrarEncomienda/ModalRegistrarEncomienda";
import MarkEmailReadIcon from '@mui/icons-material/MarkEmailRead';
import ModalRetirarEncomienda from "../../Modal/ModalRetirarEncomienda/ModalRetirarEncomienda";

const GestionEncomiendasPageComponent = () => {

    // Hook para obtener las encomiendas del usuario
    const { loading, response, responseStatus, getAllEncomiendas, encomiendas } = UseEncomienda();

    // Hook para obtener información de los departamentos del usuario
    const {getAllDepartamentos, departamentos} = useDepartamento();

    // Se obtienen las encomiendas desde el hook
    useEffect(() => {
      getAllEncomiendas();
      getAllDepartamentos();
    }, []);

    // Estado en donde se guardarán los datos de las encomiendas
    const [rowsOriginales, setRowsOriginales] = useState();
    useEffect(() => {
        if (!Array.isArray(encomiendas)) return;
    
        const encomiendasOrdenadasPorFecha = [...encomiendas].sort((a, b) =>
        dayjs(b.receivedAt).valueOf() - dayjs(a.receivedAt).valueOf()
        );
        setRowsOriginales(encomiendasOrdenadasPorFecha);
    }, [encomiendas]);

    // Estado y funciones para manejar las filas modificables de la tabla
    // Se utiliza para ordenar las encomiendas por fecha de llegada
    const [rowsModificables, setRowsModificables] = useState();
    useEffect(() => {
        if (!Array.isArray(rowsOriginales)) return;
    
        const encomiendasOrdenadasPorFecha = [...rowsOriginales].sort((a, b) =>
        dayjs(b.receivedAt).valueOf() - dayjs(a.receivedAt).valueOf()
        );
        setRowsModificables(encomiendasOrdenadasPorFecha);
    }, [rowsOriginales]);
    

    // Estado y funciones para manejar el modal de ver detalles de encomienda
    const [openModalVerDetallesEncomienda, setOpenModalVerDetallesEncomienda] = useState(false);
    const handleCloseModalVerDetallesEncomienda = () => setOpenModalVerDetallesEncomienda(false);
    const handleOpenModalVerDetallesEncomienda = (encomienda = {}) => {
      setOpenModalVerDetallesEncomienda(true);
      setEncomiendaSeleccionada(encomienda);
    };

    // Estado y funciones para manejar el modal de ver registrar encomienda
    const [openModalRegistrarEncomienda, setOpenModalRegistrarEncomienda] = useState(false);
    const handleCloseModalRegistrarEncomienda = () => setOpenModalRegistrarEncomienda(false);
    const handleOpenModalRegistrarEncomienda = () => setOpenModalRegistrarEncomienda(true);  

    // Estado y funciones para manejar el modal de retirar encomienda
    // Este modal se utiliza para marcar una encomienda como retirada
    const [openModalRetirarEncomienda, setOpenModalRetirarEncomienda] = useState(false);
    const handleCloseModalRetirarEncomienda = () => setOpenModalRetirarEncomienda(false);
    const handleOpenModalRetirarEncomienda = (encomienda = {}) => {
        setOpenModalRetirarEncomienda(true);
        setEncomiendaSeleccionada(encomienda);
    };

    // Departamento perteneciente a la encomienda seleccionada
    const [departamentoSeleccionado, setDepartamentoSeleccionado] = useState({});

    // Encomienda seleccionada para ver detalles
    const [encomiendaSeleccionada, setEncomiendaSeleccionada] = useState({});

    useEffect(() => {
    // En cuanto cambie la encomienda seleccionada, se cambia el departamento seleccionado
        const departamento = departamentos?.find((departamento) => departamento.id === encomiendaSeleccionada.idApartment);
        setDepartamentoSeleccionado(departamento || {});
    }, [encomiendaSeleccionada]);

    // Columnas de la tabla de encomiendas
    const columns = [
        "Destino",
        "Fecha de llegada",
        "Estado",
        "Acciones"
    ];

    // Filas de la tabla de encomiendas
    const data = rowsModificables?.map((encomienda) => {
        const {receivedAt, deliveredAt} = encomienda;
        const destino = departamentos?.find((departamento) => departamento.id === encomienda.idApartment) || "Desconocido";
        const {zoneName: zonaDestino, name: nombreDepartamento} = destino;
        return[
            `${zonaDestino} - ${nombreDepartamento}`,
            cambiarFormatoHoraFecha(receivedAt) || "Sin datos",
            deliveredAt ? <Chip label="Retirada" color="success" /> : <Chip label="Pendiente" color="error" />,
            <Box id="BoxAccionesTablaUltimosRegistros">
                <TooltipTipoUno titulo={"Ver detalles"} ubicacion={"right"}>
                <IconButton onClick={()=>{handleOpenModalVerDetallesEncomienda(encomienda)}}>
                    <InfoIcon id="BotonVerDetallesRegistro" fontSize="large" />
                </IconButton>
                </TooltipTipoUno>

                {deliveredAt ?
                    null
                    :
                    <TooltipTipoUno titulo={"Marcar como entregada"} ubicacion={"right"}>
                        <IconButton onClick={()=>{handleOpenModalRetirarEncomienda(encomienda)}}>
                            <MarkEmailReadIcon id="BotonVerDetallesRegistro" fontSize="large" />
                        </IconButton>
                    </TooltipTipoUno>  
                }
            </Box>
        ]
    })
    
  return (
    <Box id="ContainerGestionEncomiendasPageComponent">
        <Box id="BotonRegistrarNuevaEncomienda">
            <ButtonTypeOne
                defaultText="Registrar nueva encomienda"
                handleClick={()=>{handleOpenModalRegistrarEncomienda()}}
            />
        </Box>
        <Fade in={!(!Array.isArray(rowsOriginales))} timeout={{ enter: 500, exit: 300 }} unmountOnExit>
            <div>
                <DatagridResponsive title="Encomiendas" columns={columns} data={data} selectableRows="none" downloadCsvButton={false} />
                <ModalVerDetallesEncomienda
                    open={openModalVerDetallesEncomienda}
                    onClose={handleCloseModalVerDetallesEncomienda}
                    encomiendaSeleccionada={encomiendaSeleccionada}
                    departamentoSeleccionado={departamentoSeleccionado}
                />
                <ModalRegistarEncomienda
                    open={openModalRegistrarEncomienda}
                    onClose={handleCloseModalRegistrarEncomienda}
                    setRows={setRowsOriginales}
                    departamentos={departamentos}
                />

                <ModalRetirarEncomienda
                    open={openModalRetirarEncomienda}
                    onClose={handleCloseModalRetirarEncomienda}
                    setRows={setRowsOriginales}
                    encomiendaSeleccionada={encomiendaSeleccionada}
                    setEncomiendaSeleccionada={setEncomiendaSeleccionada}
                    departamentoSeleccionado={departamentoSeleccionado}
                />
                {/* {ConfirmDialogComponent}
                <ModalLoadingMasRespuesta
                    open={openLoadingRespuesta}
                    loading={loadingEstacionamientos}
                    message={messageLoadingRespuesta}
                    loadingMessage="Cambiando disponibilidad de estacionamiento..."
                    successfulProcess={operacionExitosa}
                    accionPostCierre={accionPostCierreLoadingRespuesta}
                /> */}
{/* 
                <ModalCrearEstacionamiento
                    open={openModalCrearEstacionamiento}
                    onClose={handleCloseModalCrearEstacionamiento}
                    setRows={setRows}
                /> */}
            </div>
        </Fade>

        <Fade in={!Array.isArray(rowsOriginales)} timeout={{ enter: 500, exit: 300 }} unmountOnExit>
            <div>
                <TableSkeleton columnCount={3} rowCount={7} />
            </div>
        </Fade>
    </Box>
  )
}

export default GestionEncomiendasPageComponent;