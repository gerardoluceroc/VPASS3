import { useEffect, useState } from "react";
import useListaNegra from "../../../hooks/useListaNegra/useListaNegra";
import "./ListaNegraPageComponent.css";
import { useConfirmDialog } from "../../../hooks/useConfirmDialog/useConfirmDialog";
import DatagridResponsive from "../../Datagrid/DatagridResponsive/DatagridResponsive";
import { Box, Fade } from "@mui/material";
import ButtonTypeOne from "../../Buttons/ButtonTypeOne/ButtonTypeOne";
import ModalLoadingMasRespuesta from "../../Modal/ModalLoadingMasRespuesta/ModalLoadingMasRespuesta";
import TableSkeleton from "../../Skeleton/TableSkeleton/TableSkeleton";
import ModalAgregarAListaNegra from "../../Modal/ModalAgregarAListaNegra/ModalAgregarAListaNegra";
import { IconoBorrar } from "../../IconButtons/IconButtons";
import { useSelector } from "react-redux";

const ListaNegraPageComponent = () => {

   // Se obtienen las funciones y estados a utilizar del hook
  const {listaNegra, getAllListaNegra, loading: loadingListaNegra, BorrarDeListaNegraPorIdPersona} = useListaNegra();

  const { idEstablishment } = useSelector((state) => state.user);

  useEffect(() => {
    getAllListaNegra();
  }, []);

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
  const handleBorrarPersonaDeListaNegra = async (persona) => {

    const {id: idPersona} = persona;

    const confirmed = await confirm({
        title: "Quitar persona",
        message: "¿Deseas quitar a esta persona de la lista negra del establecimiento?"
    });

    if(confirmed){
      setOpenLoadingRespuesta(true);

      // Se realiza la peticion al servidor para actualizar la lista negra borrando a la persona seleccionada
      const {statusCode: statusBorrarDeListaNegra, message: messageBorrarDeListaNegra} = await BorrarDeListaNegraPorIdPersona(
        {
          idEstablecimiento: idEstablishment,
          idPersona: idPersona
        }
      );

      // Si el servidor responde con el Response dto que tiene configurado
      if(statusBorrarDeListaNegra != null && statusBorrarDeListaNegra != undefined){

          if (statusBorrarDeListaNegra === 200 || statusBorrarDeListaNegra === 201 && (statusBorrarDeListaNegra != null && statusBorrarDeListaNegra != undefined)) {
              setOperacionExitosa(true);
              setMessageLoadingRespuesta(messageBorrarDeListaNegra);

              // Se actualizan las rows eliminando aquella persona seleccionada
              setRows(prevRows => prevRows.filter(row => row?.visitor?.id !== idPersona));
          }
          else if (statusBorrarDeListaNegra === 500) {
              //En caso de error 500, se muestra un mensaje de error genérico, en vez del mensaje de error del backend
              setOperacionExitosa(false);
              setMessageLoadingRespuesta("Error desconocido, por favor intente nuevamente más tarde.");
          }
          else{
              //En caso de cualquier otro error, se muestra el mensaje de error del backend
              setOperacionExitosa(false);
              setMessageLoadingRespuesta(messageBorrarDeListaNegra);
          }
      }
      else{
        //Esto es para los casos que el servidor no responda el ResponseDto tipico
        setOperacionExitosa(false);
        setMessageLoadingRespuesta("Error desconocido, por favor intente nuevamente más tarde.");
      }
    }
  }

  // Estado en donde se guardarán los datos de la lista negra, es con el objetivo de manipular el arreglo
  const [rows, setRows] = useState();

  // En el momento en que carguen los datos de la lista negra, se hace una copia para rows.
  useEffect(() => {
      if (!Array.isArray(listaNegra)) return;
      setRows(listaNegra);
  }, [listaNegra]);

  // Información que irá en la tabla
  const columns = ["Nombres", "Apellidos", "RUT/Pasaporte", "Motivo", "Acciones"];
  const data = rows?.map((listaNegra) => {
      const {reason, visitor} = listaNegra;
      const {names = "", lastNames = "", identificationNumber} = visitor;
      const columnaAcciones = 
      <Box>
          <IconoBorrar handleClick={()=>handleBorrarPersonaDeListaNegra(visitor)}/>
      </Box>;

      return[
          `${names}`.trim(),
          `${lastNames}`.trim(),
          `${identificationNumber}`,
          `${reason}`,
          columnaAcciones
      ]
  })

  const [openModalCrearListaNegra, setOpenModalCrearListaNegra] = useState(false);
  const handleOpenModalCrearListaNegra = () => setOpenModalCrearListaNegra(true);  
  const handleCloseModalCrearListaNegra = () => setOpenModalCrearListaNegra(false);

  return (
    <Box id="ContainerListaNegraPageComponent">
      <Box id="BotonCrearNuevaListaNegra">
          <ButtonTypeOne
              defaultText="Agregar visitante a lista negra"
              handleClick={handleOpenModalCrearListaNegra}
          />
      </Box>
      <Fade in={!(!Array.isArray(rows))} timeout={{ enter: 500, exit: 300 }} unmountOnExit>
        <div>
          <DatagridResponsive title="Lista Negra" columns={columns} data={data} selectableRows="none" downloadCsvButton={false} /> 
          {ConfirmDialogComponent}
          <ModalAgregarAListaNegra
            open={openModalCrearListaNegra}
            onClose={handleCloseModalCrearListaNegra}
            setRows={setRows}
          />

          <ModalLoadingMasRespuesta
              open={openLoadingRespuesta}
              loading={loadingListaNegra}
              message={messageLoadingRespuesta}
              loadingMessage="Eliminando persona de lista negra..."
              successfulProcess={operacionExitosa}
              accionPostCierre={accionPostCierreLoadingRespuesta}
          />
        </div>
      </Fade>

      <Fade in={!Array.isArray(rows)} timeout={{ enter: 500, exit: 300 }} unmountOnExit>
          <div>
              <TableSkeleton columnCount={3} rowCount={7} />
          </div>
      </Fade>
    </Box>
  )
}

export default ListaNegraPageComponent;