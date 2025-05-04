import { Box } from "@mui/material";
import Dashboard from "../../Dashboard/Dashboard";
import "./HomePageComponent.css";

function HomePageComponent() {

  return (
    <Box id="ContainerHomePageComponent">
      <Dashboard/> 
    </Box>
  );
}

export default HomePageComponent;



























































































// import { AppBar, Box, ButtonBase, Card, CardMedia, CssBaseline, Divider, Drawer, IconButton, List, ListItem, ListItemButton, ListItemIcon, ListItemText, Toolbar, Typography } from "@mui/material";
// import InboxIcon from '@mui/icons-material/MoveToInbox';
// import MailIcon from '@mui/icons-material/Mail';
// import MenuIcon from '@mui/icons-material/Menu';
// import { useState } from "react";
// import VPassImage from '../../assets/VpassWhite.jpg'
// import { useNavigate } from "react-router-dom";
// import "./HomeComponent.css";
// import Dashboard from "../Dashboard/Dashboard";

// const drawerWidth = 280;

// function HomeComponent(props) {
//   const { window } = props;
//   const [mobileOpen, setMobileOpen] = useState(false);
//   const [isClosing, setIsClosing] = useState(false);
//   const navigate = useNavigate();

//   const opcionesDrawner = [
//     {
//       nombre: "Panel de control",
//       icono: <InboxIcon />,
//     },
//     {
//       nombre: "Últimos registros",
//       icono: <MailIcon />,
//     },
//     {
//       nombre: "Blacklist",
//       icono: <InboxIcon />,
//     },
//     {
//       nombre: "Descargar registros",
//       icono: <MailIcon />,
//     },
//     {
//       nombre: "Enviar reporte rápido",
//       icono: <InboxIcon />,
//     },
// ]

//   const handleDrawerClose = () => {
//     setIsClosing(true);
//     setMobileOpen(false);
//   };

//   const handleDrawerTransitionEnd = () => {
//     setIsClosing(false);
//   };

//   const handleDrawerToggle = () => {
//     if (!isClosing) {
//       setMobileOpen(!mobileOpen);
//     }
//   };

//   const drawer = (
//     <div id="ContainerDrawnerHomeComponent">

//       <ButtonBase onClick={()=>navigate("/", { replace: true })}>
//         <Box
//           component="img"
//           src={VPassImage}
//           alt="Descripción"
//           id="ImagenLogoVpassHomeComponent"
//         />
//       </ButtonBase>
//       <Divider className="DividerDrawnerHomeComponent" />
//       <List>
//         {opcionesDrawner.map((opcion, index) => (
//           <ListItem key={opcion.nombre} disablePadding>
//             <ListItemButton>
//               <ListItemIcon>
//                 {opcion.icono}
//               </ListItemIcon>
//               <ListItemText primary={opcion.nombre} />
//             </ListItemButton>
//           </ListItem>
//         ))}
//       </List>
//       <Divider className="DividerDrawnerHomeComponent" />
//       <List>
//         {['All mail', 'Trash', 'Spam'].map((text, index) => (
//           <ListItem key={text} disablePadding>
//             <ListItemButton>
//               <ListItemIcon>
//                 {index % 2 === 0 ? <InboxIcon /> : <MailIcon />}
//               </ListItemIcon>
//               <ListItemText primary={text} />
//             </ListItemButton>
//           </ListItem>
//         ))}
//       </List>
//     </div>
//   );

//   return (
//     <Box sx={{ display: 'flex' }}>
//       <CssBaseline />
//       <AppBar
//         position="fixed"
//         sx={{
//           // width: { sm: `calc(100% - ${drawerWidth}px)` },
//           width:"100%",
//           ml: { sm: `${drawerWidth}px` },
//           backgroundColor: "#2f4c78",

//         }}
//       >
//         <Toolbar>
//           <IconButton
//             color="inherit"
//             aria-label="open drawer"
//             edge="start"
//             onClick={handleDrawerToggle}
//             sx={{ mr: 2, display: { sm: 'none' } }}
//           >
//             <MenuIcon />
//           </IconButton>
//         </Toolbar>
//       </AppBar>

//       <Box
//         component="nav"
//         sx={{ width: { sm: drawerWidth }, flexShrink: { sm: 0 } }}
//         aria-label="mailbox folders"
//       >
//         {/* Drawner para vista movil */}
//         <Drawer
//           // container={container}
//           variant="temporary"
//           open={mobileOpen}
//           onTransitionEnd={handleDrawerTransitionEnd}
//           onClose={handleDrawerClose}
//           sx={{
//             display: { xs: 'block', sm: 'none' },
//             '& .MuiDrawer-paper': { boxSizing: 'border-box', width: drawerWidth },
//           }}
//           slotProps={{
//             root: {
//               keepMounted: true, // Better open performance on mobile.
//             },
//           }}
//         >
//           {drawer}
//         </Drawer>

//         {/* Drawner para vista escritorio */}
//         <Drawer
//           variant="permanent"
//           sx={{
//             display: { xs: 'none', sm: 'block' },
//             '& .MuiDrawer-paper': { boxSizing: 'border-box', width: drawerWidth },
//           }}
//           open
//         >
//           {drawer}
//         </Drawer>
//       </Box>

      
//       <Box
//         component="main"
//         sx={{ flexGrow: 1, p: "0px 20px 20px 30px", width: { sm: `calc(100% - ${drawerWidth}px)` }, minHeight: "100vh" }}
//         // sx={{ flexGrow: 1, p: 3, height: "100%", backgroundColor: "red" }}
//       >
//         <Toolbar />
//         <Box id="CuerpoHomeComponent">

//           <input type="text" placeholder="Buscar" id="InputBuscarHomeComponent" />
//           <Dashboard/>
//         </Box>
//       </Box>
//     </Box>
//   );
// }

// export default HomeComponent;