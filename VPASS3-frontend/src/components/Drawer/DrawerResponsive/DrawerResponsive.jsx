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
  } from "@mui/material";
  import MenuIcon from "@mui/icons-material/Menu";
  import { useState } from "react";
  import { useNavigate } from "react-router-dom";
  import VPassImage from "../../../assets/VpassWhite.jpg";
  import "./DrawerResponsive.css";
  import { RUTA_BITACORA_INCIDENCIAS, RUTA_BITACORA_USO_ESTACIONAMIENTO, RUTA_DESCARGAR_REGISTROS, RUTA_GESTION_ESTACIONAMIENTO, RUTA_GESTION_ZONAS, RUTA_HOME } from "../../../utils/rutasCliente";
  import DirectionsCarFilledIcon from '@mui/icons-material/DirectionsCarFilled';
  import DashboardIcon from '@mui/icons-material/Dashboard';
  import { IconoLogs } from "../../../icons/iconos";
  import SimCardDownloadIcon from '@mui/icons-material/SimCardDownload';
  import InboxIcon from "@mui/icons-material/MoveToInbox";
  import MailIcon from "@mui/icons-material/Mail";
  import LocalParkingIcon from '@mui/icons-material/LocalParking';
  import LocationCityIcon from '@mui/icons-material/LocationCity';
    
  const drawerWidth = 280;
  
  const opcionesDrawner = [
    { 
      nombre: "Panel de control", 
      icono: <DashboardIcon sx={{color: "white"}} />,
      ruta: RUTA_HOME
    },
    { 
      nombre: "Gestion de estacionamientos", 
      icono: <DirectionsCarFilledIcon sx={{color: "white"}} />,
      ruta: RUTA_GESTION_ESTACIONAMIENTO
    },
    { 
      nombre: "Bitácora de incidencias", 
      icono: <IconoLogs />,
      ruta: RUTA_BITACORA_INCIDENCIAS
    },
    { 
      nombre: "Gestion de zonas", 
      icono: <LocationCityIcon sx={{color: "white"}} />,
      ruta: RUTA_GESTION_ZONAS
    },
    { 
      nombre: "Bitácora de uso de estacionamiento", 
      icono: <LocalParkingIcon sx={{color: "white"}} />, 
      ruta: RUTA_BITACORA_USO_ESTACIONAMIENTO
    },
  ];
  
  export default function DrawerResponsive({ children, handleOpcionSeleccionada }) {
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

    const navigate = useNavigate();

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
        <List>
          {opcionesDrawner.map((opcion) => (
            <ListItem key={opcion.nombre} sx={{color: "white"}} disablePadding>
              <ListItemButton onClick={()=>navigate(opcion.ruta)}>
                <ListItemIcon>{opcion.icono}</ListItemIcon>
                <ListItemText primary={opcion.nombre} />
              </ListItemButton>
            </ListItem>
          ))}
        </List>
        <Divider className="DividerDrawerResponsive" />
        {/* <Box sx={{backgroundColor: "red", display: "flex", flexDirection: "column", justifyContent: "space-between", height: "100%"}}>
          <button>Click aca</button>
          <button>Click aca</button>
        </Box> */}
        <List>
          {["All mail", "Trash", "Spam"].map((text, index) => (
            <ListItem key={text} disablePadding>
              <ListItemButton>
                <ListItemIcon>
                  {index % 2 === 0 ? <InboxIcon /> : <MailIcon />}
                </ListItemIcon>
                <ListItemText primary={text} />
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
  