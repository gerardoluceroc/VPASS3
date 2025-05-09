import { SvgIcon } from "@mui/material";

export function IconoExito(props) {
    return (
        <SvgIcon {...props}>
            <svg xmlns="http://www.w3.org/2000/svg" width="104" height="104" viewBox="0 0 104 104" fill="none">
                <g filter="url(#filter0_dd_3155_43455)">
                    <path d="M76.4444 36.3333L42.2221 70.5557L27.5553 55.8889M96 51C96 75.3005 76.3005 95 52 95C27.6995 95 8 75.3005 8 51C8 26.6995 27.6995 7 52 7C76.3005 7 96 26.6995 96 51Z" stroke="#338333" strokeWidth="8" strokeLinecap="round" strokeLinejoin="round"/>
                </g>
                <defs>
                    <filter id="filter0_dd_3155_43455" x="0" y="0" width="104" height="104" filterUnits="userSpaceOnUse" colorInterpolationFilters="sRGB">
                    <feFlood floodOpacity="0" result="BackgroundImageFix"/>
                    <feColorMatrix in="SourceAlpha" type="matrix" values="0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 127 0" result="hardAlpha"/>
                    <feMorphology radius="1" operator="dilate" in="SourceAlpha" result="effect1_dropShadow_3155_43455"/>
                    <feOffset dy="1"/>
                    <feGaussianBlur stdDeviation="1.5"/>
                    <feColorMatrix type="matrix" values="0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0.15 0"/>
                    <feBlend mode="normal" in2="BackgroundImageFix" result="effect1_dropShadow_3155_43455"/>
                    <feColorMatrix in="SourceAlpha" type="matrix" values="0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 127 0" result="hardAlpha"/>
                    <feOffset dy="1"/>
                    <feGaussianBlur stdDeviation="1"/>
                    <feColorMatrix type="matrix" values="0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0.3 0"/>
                    <feBlend mode="normal" in2="effect1_dropShadow_3155_43455" result="effect2_dropShadow_3155_43455"/>
                    <feBlend mode="normal" in="SourceGraphic" in2="effect2_dropShadow_3155_43455" result="shape"/>
                    </filter>
                </defs>
            </svg>
        </SvgIcon>
    );
}

export function IconoFallo(props) {
    return (
        <SvgIcon {...props}>
            <svg width="96" height="96" viewBox="0 0 96 96" fill="none" xmlns="http://www.w3.org/2000/svg">
                <path id="shape" d="M67.5555 28.4444L28.4444 67.5556M28.4445 28.4444L67.5556 67.5556M92 48C92 72.3005 72.3005 92 48 92C23.6995 92 4 72.3005 4 48C4 23.6995 23.6995 4 48 4C72.3005 4 92 23.6995 92 48Z" stroke="#CC261A" strokeWidth="8" strokeLinecap="round" strokeLinejoin="round"/>
            </svg>
        </SvgIcon>
    );
}