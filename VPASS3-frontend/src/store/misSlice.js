import { createSlice } from "@reduxjs/toolkit";

const authSlice = createSlice(
    {
        name: "authentication",
        initialState:
        {
            authenticated: false,
            token: null,
            rememberMe: false,
            idEstablishment: null,
            expirationTokenTimestamp: null,
        },
        reducers:
        {
            setUser: (state, action) => {
                state.authenticated = action.payload.authenticated;
                state.token = action.payload.token;
                state.rememberMe = action.payload.rememberMe;
                state.expirationTokenTimestamp = action.payload.expirationTokenTimestamp;
                state.idEstablishment = parseInt(action.payload.idEstablishment);
            },
            disconnect: (state) => {
                state.authenticated = false;
                state.token = null;
                state.rememberMe = false;
                state.expirationTokenTimestamp = null;
                state.idEstablishment = null;
            },            
        }
    }
)
export const { setUser, disconnect } = authSlice.actions;
export const authReducer = authSlice.reducer;