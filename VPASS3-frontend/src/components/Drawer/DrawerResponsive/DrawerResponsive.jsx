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
  import InboxIcon from "@mui/icons-material/MoveToInbox";
  import MailIcon from "@mui/icons-material/Mail";
  import { useState } from "react";
  import { useNavigate } from "react-router-dom";
  import VPassImage from "../../../assets/VpassWhite.jpg";
  import "./DrawerResponsive.css";
import { opcionPanelDeControl } from "../../Home/constantesHome";
  
  const drawerWidth = 280;
  
  const opcionesDrawner = [
    { nombre: "Panel de control", icono: <InboxIcon /> },
    { nombre: "Últimos registros", icono: <MailIcon /> },
    { nombre: "Blacklist", icono: <InboxIcon /> },
    { nombre: "Descargar registros", icono: <MailIcon /> },
    { nombre: "Enviar reporte rápido", icono: <InboxIcon /> },
  ];
  
  export default function DrawerResponsive({ children, handleOpcionSeleccionada }) {
    const [mobileOpen, setMobileOpen] = useState(false);
    const [isClosing, setIsClosing] = useState(false);
    const navigate = useNavigate();
  
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
  
    const drawer = (
      <div id="ContainerDrawerResponsive">
        <ButtonBase onClick={() => { 
                handleOpcionSeleccionada(opcionPanelDeControl);
                handleDrawerClose(); // Cierra el menú al seleccionar una opción
                navigate("/", { replace: true });
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
            <ListItem key={opcion.nombre} disablePadding>
              <ListItemButton>
                <ListItemIcon>{opcion.icono}</ListItemIcon>
                <ListItemText primary={opcion.nombre} />
              </ListItemButton>
            </ListItem>
          ))}
        </List>
        <Divider className="DividerDrawerResponsive" />
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
            p: "0px 20px 20px 30px",
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
  