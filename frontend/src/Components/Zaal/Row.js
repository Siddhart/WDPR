import Seat from './Seat'

export default function Row({ nmr,seats }) {

    return (
        <div className="flex gap-2">
            <p className="font-bold text-xl w-8 flex justify-center items-center">{nmr + 1}</p>
            <div className="flex gap-2 w-72 justify-center">
                {seats.map(seat => {
                    return <Seat key={Math.random()} type={seat}  />
                })}
            </div>
        </div>
    )
}