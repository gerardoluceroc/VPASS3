import { createSlice } from "@reduxjs/toolkit";

const authSlice = createSlice(
    {
        name: "authentication",
        initialState:
        {
            authenticated: false,
            token: null,
            expiracion : null,
            idEmpresa: null,
            usuario: null,
            correo: null,
            idPersona: null,
            rol: null,
            rememberMe: false,
        },
        reducers:
        {
            setUser: (state, action) => {
                state.authenticated = action.payload;
                state.token = action.payload;
                state.expiracion = action.payload;
                state.idEmpresa = action.payload;
                state.usuario = action.payload;
                state.correo = action.payload;
                state.idPersona = action.payload;
                state.rol = action.payload;
                state.rememberMe = action.payload;
            },
            disconnect: (state, action) => {
                state.authenticated = action.payload;
                state.token = action.payload;
                state.expiracion = action.payload;
                state.idEmpresa = action.payload;
                state.usuario = action.payload;
                state.correo = action.payload;
                state.idPersona = action.payload;
                state.rol = action.payload;
                state.rememberMe = action.payload;
            }
        }
    }
)
export const { setUser, disconnect } = authSlice.actions;
export const authReducer = authSlice.reducer;





// import { createSlice } from "@reduxjs/toolkit";

// const authSlice = createSlice({
//     name: "authentication", // Nombre del slice
//     initialState: {
//         authenticated: false,
//         token: null,
//         expiracion: null,
//         idEmpresa: null,
//         usuario: null,
//         correo: null,
//         idPersona: null,
//         rol: null,
//         rememberMe: false,
//     },
//     reducers: {
//         setUser: (state, action) => {
//             // Acá estás reemplazando todo con el mismo objeto payload
//             Object.assign(state, action.payload); // Mejor alternativa
//         },
//         disconnect: (state, action) => {
//             // Lo mismo aquí, deberías dejar el estado limpio
//             return {
//                 authenticated: false,
//                 token: null,
//                 expiracion: null,
//                 idEmpresa: null,
//                 usuario: null,
//                 correo: null,
//                 idPersona: null,
//                 rol: null,
//                 rememberMe: false,
//             };
//         }
//     }
// });

// export const { setUser, disconnect } = authSlice.actions;
// export const authReducer = authSlice.reducer;
