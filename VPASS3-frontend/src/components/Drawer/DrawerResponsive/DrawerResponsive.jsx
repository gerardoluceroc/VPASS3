import {
    AppBar,
    Box,
    CssBaseline,
    Divider,
    Drawer,
    IconButton,
    List,
    ListItem,
    ListItemButton,
    ListItemIcon,
    ListItemText,
    Toolbar,
    ButtonBase,
    Typography,
  } from "@mui/material";
  import MenuIcon from "@mui/icons-material/Menu";
  import { useState } from "react";
  import { useNavigate } from "react-router-dom";
  import VPassImage from "../../../assets/VpassWhite.jpg";
  import "./DrawerResponsive.css";
  import { RUTA_BITACORA_INCIDENCIAS, RUTA_BITACORA_USO_ESTACIONAMIENTO, RUTA_DESCARGAR_REGISTROS, RUTA_GESTION_ENCOMIENDAS, RUTA_GESTION_ESPACIOS_COMUNES, RUTA_GESTION_ESTACIONAMIENTO, RUTA_GESTION_ZONAS, RUTA_HOME, RUTA_LOGIN } from "../../../utils/rutasCliente";
  import DirectionsCarFilledIcon from '@mui/icons-material/DirectionsCarFilled';
  import DashboardIcon from '@mui/icons-material/Dashboard';
  import { IconoLogs } from "../../../icons/iconos";
  import LocalParkingIcon from '@mui/icons-material/LocalParking';
  import LocationCityIcon from '@mui/icons-material/LocationCity';
  import WorkspacesIcon from '@mui/icons-material/Workspaces';
  import PersonIcon from '@mui/icons-material/Person';
  import MarkunreadMailboxIcon from '@mui/icons-material/MarkunreadMailbox';
  import { useSelector } from "react-redux";
import useLogin from "../../../hooks/auth/useLogin";
import { useConfirmDialog } from "../../../hooks/useConfirmDialog/useConfirmDialog";
    
  const drawerWidth = 280;
  
  const opcionesDrawner = [
    { 
      id: 1,
      nombre: "Panel de control", 
      icono: <DashboardIcon sx={{color: "white"}} />,
      ruta: RUTA_HOME
    },
    { 
      id: 2,
      nombre: "Gestion de estacionamientos", 
      icono: <DirectionsCarFilledIcon sx={{color: "white"}} />,
      ruta: RUTA_GESTION_ESTACIONAMIENTO
    },
    { 
      id: 3,
      nombre: "Bitácora de incidencias", 
      icono: <IconoLogs />,
      ruta: RUTA_BITACORA_INCIDENCIAS
    },
    { 
      id: 4,
      nombre: "Gestion de zonas", 
      icono: <LocationCityIcon sx={{color: "white"}} />,
      ruta: RUTA_GESTION_ZONAS
    },
    { 
      id: 5,
      nombre: "Bitácora de uso de estacionamiento", 
      icono: <LocalParkingIcon sx={{color: "white"}} />, 
      ruta: RUTA_BITACORA_USO_ESTACIONAMIENTO
    },
    { 
      id: 6,
      nombre: "Gestión de espacios comunes",
      icono: <WorkspacesIcon sx={{color: "white"}} />, 
      ruta: RUTA_GESTION_ESPACIOS_COMUNES
    },
    { 
      id: 7,
      nombre: "Gestión de encomiendas",
      icono: <MarkunreadMailboxIcon sx={{color: "white"}} />, 
      ruta: RUTA_GESTION_ENCOMIENDAS
    },
  ];
  
  export default function DrawerResponsive({ 
    children, 
  }) {
    // Se invoca la función para consultarle al usuario si desea cerrar sesión
    const { confirm, ConfirmDialogComponent } = useConfirmDialog();

    const { email: emailUsuario = ""} = useSelector((state) => state.user);
    const {logoutSession} = useLogin();
    const navigate = useNavigate();

    const menuInferiorOpciones = [
    {
      id: 1,
      texto: "Cerrar sesión",
      icono: <PersonIcon sx={{color: "white"}} />,
      accion: async () => {

        const confirmed = await confirm({
            title: "Cerrar sesión",
            message: "¿Estás seguro de que deseas cerrar sesión?"
        });

        if(confirmed) {
          await logoutSession();
          navigate(RUTA_LOGIN);
        }
      }
    }
  ];

    const [mobileOpen, setMobileOpen] = useState(false);
    const [isClosing, setIsClosing] = useState(false);

    const handleDrawerToggle = () => {
      if (!isClosing) {
        setMobileOpen(!mobileOpen);
      }
    };
  
    const handleDrawerClose = () => {
      setIsClosing(true);
      setMobileOpen(false);
    };
  
    const handleDrawerTransitionEnd = () => {
      setIsClosing(false);
    };

    const handleOpcionClick = (ruta) => {
        navigate(ruta);
    };
  
    const drawer = (
      <div id="ContainerDrawerResponsive">
        <ButtonBase onClick={() => { 
                handleDrawerClose(); // Cierra el menú al seleccionar una opción
                handleOpcionClick(RUTA_HOME);
            }
            }>
          <Box
            component="img"
            src={VPassImage}
            alt="Logo"
            id="ImagenLogoVpassDrawerResponsive"
          />
        </ButtonBase>
        <Divider className="DividerDrawerResponsive" />
        <List id="MenuSuperiorOpcionesDrawerResponsive">
          {opcionesDrawner.map((opcion) => (
            <ListItem key={opcion.id} sx={{color: "white"}} disablePadding>
              <ListItemButton onClick={()=>navigate(opcion.ruta)}>
                <ListItemIcon>{opcion.icono}</ListItemIcon>
                <ListItemText primary={opcion.nombre} />
              </ListItemButton>
            </ListItem>
          ))}
        </List>
        <Divider className="DividerDrawerResponsive" />
        <List id="MenuInferiorOpcionesDrawerResponsive">
          <Typography variant="subtitle1" sx={{display: "flex", alignItems: "center", justifyContent: "center", color: "white", padding: "0px 20px 20px 20px"}}>
            {`Bienvenido ${emailUsuario}`}
          </Typography>
          {menuInferiorOpciones.map((opcion) => (
            <ListItem key={opcion.id} sx={{color: "white"}} disablePadding>
              <ListItemButton onClick={opcion.accion}>
                <ListItemIcon>
                  {opcion.icono}
                </ListItemIcon>
                <ListItemText primary={opcion.texto} />
              </ListItemButton>
            </ListItem>
          ))}
        </List>
      </div>
    );
  
    return (
      <Box sx={{ display: "flex" }}>
        <CssBaseline />
        <AppBar
          position="fixed"
          sx={{
            width: "100%",
            ml: { sm: `${drawerWidth}px` },
            backgroundColor: "#2f4c78",
          }}
        >
          <Toolbar>
            <IconButton
              color="inherit"
              edge="start"
              onClick={handleDrawerToggle}
              sx={{ mr: 2, display: { sm: "none" } }}
            >
              <MenuIcon />
            </IconButton>
          </Toolbar>
          {ConfirmDialogComponent}
        </AppBar>
  
        {/* Menú lateral para móvil */}
        <Box
          component="nav"
          sx={{ width: { sm: drawerWidth }, flexShrink: { sm: 0 } }}
          aria-label="mailbox folders"
        >
          <Drawer
            variant="temporary"
            open={mobileOpen}
            onClose={handleDrawerClose}
            onTransitionEnd={handleDrawerTransitionEnd}
            sx={{
              display: { xs: "block", sm: "none" },
              "& .MuiDrawer-paper": {
                boxSizing: "border-box",
                width: drawerWidth,
              },
            }}
            slotProps={{
              root: {
                keepMounted: true,
              },
            }}
          >
            {drawer}
          </Drawer>
  
          {/* Menú lateral para escritorio */}
          <Drawer
            variant="permanent"
            sx={{
              display: { xs: "none", sm: "block" },
              "& .MuiDrawer-paper": {
                boxSizing: "border-box",
                width: drawerWidth,
              },
            }}
            open
          >
            {drawer}
          </Drawer>
        </Box>
  
        {/* Contenido principal */}
        <Box
          component="main"
          sx={{
            flexGrow: 1,
            width: { sm: `calc(100% - ${drawerWidth}px)` },
            minHeight: "100vh",
          }}
        >
          <Toolbar />
          {children}
        </Box>
      </Box>
    );
  }
  